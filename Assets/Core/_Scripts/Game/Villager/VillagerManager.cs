using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

// TODO : Replace some OnPopulationChanged calls by a more individual focused event
public class VillagerManager : MonoBehaviour
{
    public event Action<List<VillagerData>> OnPopulationChanged;

    private VillagerGenerator m_villagerGenerator;

    private List<VillagerData> m_population;
    private VillagerData m_currentVillager;
    private List<VillagerData> m_villagerQueue;

    private TimeManager m_timeManager;

    public ReadOnlyCollection<VillagerData> GetPopulation()
    {
        return m_population.AsReadOnly();
    }

    private void OnDestroy()
    {
        m_timeManager.OnWeekEnded -= FeedPopulation;
    }

    public void Initialize()
    {
        m_villagerGenerator = new VillagerGenerator();
        m_villagerGenerator.Initialize();

        m_population = new List<VillagerData>();
        OnPopulationChanged?.Invoke(m_population);
        m_villagerQueue = new List<VillagerData>();

        m_timeManager = GameManager.Instance.GetTimeManager();
        m_timeManager.OnWeekEnded += FeedPopulation;

#if UNITY_EDITOR
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action<int, int, int>>("initpop", (int child, int adults, int elders) =>
        {
            CreateRandomVillagers(child, adults, elders);
            ListPopulation();
        }));

        commandSystem.AddCommand(new CommandDefinition<Action>("getsick", () =>
        {
            GetSick();
        }));
        commandSystem.AddCommand(new CommandDefinition<Action>("getpregnant", () =>
        {
            GetPregnant();
        }));
        commandSystem.AddCommand(new CommandDefinition<Action>("getbaby", () =>
        {
            DeliverBaby();
        }));
        commandSystem.AddCommand(new CommandDefinition<Action>("showid", () =>
        {
            ShowIDs();
        }));
#endif
    }

    public void FeedPopulation(int week)
    {
        ResourceHandler handler = GameManager.Instance.GetResourceHandler();
        bool famine = false;
        foreach (VillagerData villager in m_population)
        {
            if (handler.HasEnoughResources(2, 0, 0) && famine == false)
            {
                handler.ConsumeRations(2);
                if (villager.HasAnyHealthStatus(VillagerData.HealthStatus.HUNGRY,
                    VillagerData.HealthStatus.STARVED))
                {
                    villager.RemoveHealthStatus(VillagerData.HealthStatus.HUNGRY);
                    villager.RemoveHealthStatus(VillagerData.HealthStatus.STARVED);
                }
            }
            else
            {
                var isHungry = villager.HasHealthStatus(VillagerData.HealthStatus.HUNGRY);
                var isStarved = villager.HasHealthStatus(VillagerData.HealthStatus.STARVED);
                if (isHungry && !isStarved)
                {
                    villager.ApplyHealthStatus(VillagerData.HealthStatus.STARVED);
                    villager.RemoveHealthStatus(VillagerData.HealthStatus.HUNGRY);
                }
                else
                {
                    if (isStarved)
                    {
                        // TODO : Kill the fucking villager
                        continue;
                    }
                    villager.ApplyHealthStatus(VillagerData.HealthStatus.HUNGRY);
                }
            }
        }

        OnPopulationChanged?.Invoke(m_population);

#if UNITY_EDITOR
        ListPopulation();
#endif
    }

    #region Villager Handling Utilities

    public void IncreaseFatigue(VillagerData data, int value)
    {
        data.IncreaseFatigue(value);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void DecreaseFatigue(VillagerData data, int value)
    {
        data.DecreaseFatigue(value);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void ApplyHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.ApplyHealthStatus(status);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void RemoveHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.RemoveHealthStatus(status);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void GetPregnant()
    {
        bool somebodyPregnant = false;
        foreach (VillagerData villager in m_population)
        {
            if (villager.IsAdult() && villager.GetGender() == VillagerData.Gender.FEMALE)
            {
                villager.Impregnate();

                Debug.Log($"is pregnant: {villager}");
                somebodyPregnant = true;
                break;
            }
        }

        if (somebodyPregnant == false)
        {
            Debug.Log("nobody was made pregnant");
        }
        else
        {
            OnPopulationChanged?.Invoke(m_population);
        }
    }

    public void DeliverBaby()
    {
        bool _isBabyBorn = false;
        foreach (VillagerData villager in m_population)
        {
            //if (villager.HasHealthStatus(VillagerData.HealthStatus.PREGNANT))
            //{
            //    //villager.RemoveHealthStatus(VillagerData.HealthStatus.PREGNANT);
            //    CreateRandomVillager();
            //    m_currentVillager.SetAge(0);
            //    Debug.Log($"{m_currentVillager} is born !");
            //    AddVillagerToPopulation();
            //    _isBabyBorn = true;
            //}
        }
        if (_isBabyBorn == false)
        {
            Debug.Log("no baby born");
        }
    }

    public void SetWorkingStatus(VillagerData data, VillagerData.WorkingStatus status)
    {
        data.SetWorkingStatus(status);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void GetSick()
    {
        var rng = GameManager.RNG;
        int randomNumber = rng.Next(0, m_population.Count);
        m_population[randomNumber].ApplyHealthStatus(VillagerData.HealthStatus.SICK);
        OnPopulationChanged?.Invoke(m_population);
        Debug.Log(m_population[randomNumber]);
        Debug.Log("is sick, yeaaah");
    }

    public void SendVillagerRepairRoom(string villagerId, string roomId)
    {
        CommandLogManager clm = FindObjectOfType<CommandLogManager>();
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        RoomData room = roomManager.GetRoomWithId(roomId);
        VillagerData villager = GetVillagerByID(villagerId);

        if (villager == null) {
            clm.AddLog($"Villager not found", GameManager.RED);
            return; 
        } // Check villager != null
        
        if (villager.GetID() == villagerId)
        {
            if (room == null)
            {
                clm.AddLog($"Room not found", GameManager.RED);
                return;
            }
            roomManager.TryToRepairRoom(villager, roomId, 5); // Check if can repair the room
        }
    }


    #endregion

    #region Villager Setup Utilities

    private void CreateRandomVillager(VillagerData.AgeStage stage)
    {
        var age = m_villagerGenerator.GenerateAgeByStage(stage);
        CreateRandomVillager(age);
    }

    private void CreateRandomVillager(int age = VillagerData.DEFAULT_AGE)
    {
        var name = m_villagerGenerator.GenerateName();
        var gender = m_villagerGenerator.SelectRandomGender();
        var personality = m_villagerGenerator.SelectRandomPersonality();

        var villager = new VillagerData.Builder(name)
            .SetAge(age)
            .SetGender(gender)
            .SetPersonality(personality)
            .Build();

        m_currentVillager = villager;
        Debug.Log($"villager created: {m_currentVillager}");
    }

    private void AssignIdentifierToVillager()
    {
        var name = m_currentVillager.GetName();
        var index = m_population.Count + 1;
        m_currentVillager.SetID(m_villagerGenerator.GenerateID(name, index));
    }

    private void AddVillagerToPopulation()
    {
        if (m_currentVillager == null)
        {
            Debug.LogError($"No villager created, use {nameof(CreateRandomVillager)} before calling this method");
            return;
        }

        if (m_population == null)
        {
            m_population = new List<VillagerData>();
        }

        AssignIdentifierToVillager();
        m_population.Add(m_currentVillager);
        OnPopulationChanged?.Invoke(m_population);
        m_currentVillager = null;
    }

    private void CreateRandomVillagers(int kids, int adults, int elders)
    {
        while (kids > 0)
        {
            CreateRandomVillager(VillagerData.AgeStage.KID);
            AddVillagerToPopulation();
            kids--;
        }

        while (adults > 0)
        {
            CreateRandomVillager(VillagerData.AgeStage.ADULT);
            AddVillagerToPopulation();
            adults--;
        }

        while (elders > 0)
        {
            CreateRandomVillager(VillagerData.AgeStage.ELDER);
            AddVillagerToPopulation();
            elders--;
        }
    }

    private void CreateRandomVillagersInQueue(int amount)
    {
        while (amount > 0)
        {
            CreateRandomVillager(m_villagerGenerator.GenerateAge());
            m_villagerQueue.Add(m_currentVillager);
            amount--;
        }
    }

    private void AddFromQueueToPopulation(int queueIndex)
    {
        if (queueIndex < 0 || queueIndex >= m_population.Count)
        {
            Debug.LogError("Index out of queue range");
            return;
        }

        VillagerData visitor = m_villagerQueue[queueIndex];
        m_villagerQueue.Remove(visitor);

        m_currentVillager = visitor;
        AssignIdentifierToVillager();
        m_currentVillager = null;

        m_population.Add(visitor);
        OnPopulationChanged?.Invoke(m_population);
    }

    //checking a villager ID with the input information

    public VillagerData GetVillagerByID(string idInput)
    {
        foreach (VillagerData villager in m_population)
        {
            if (idInput.ToUpper() == villager.GetID())
            {
                return villager;
            }
        }
        return null;
    }

    public void ShowIDs()
    {
        foreach (VillagerData villager in m_population)
        {
            Debug.Log(villager.GetID());
        }
    }

    //private void KillVillagers(int numberToKill)
    //{
    //    if (numberToKill <= m_population.Count)
    //    {
    //        Debug.Log($"is getting killed {m_population[numberToKill]})");
    //        m_population.Remove(m_population[numberToKill]);
    //    }
    //}

#if UNITY_EDITOR
    private void ListPopulation()
    {
        m_population.Print();
    }
#endif

    #endregion
}
