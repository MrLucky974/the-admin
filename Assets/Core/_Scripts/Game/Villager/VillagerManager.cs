using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;


public class VillagerManager : MonoBehaviour
{
    public event Action<List<VillagerData>> OnPopulationChanged;

    private VillagerGenerator m_villagerGenerator;

    private List<VillagerData> m_population;
    private VillagerData m_currentVillager;
    private List<VillagerData> m_villagerQueue;
    

    public ReadOnlyCollection<VillagerData> GetPopulation()
    {
        return m_population.AsReadOnly();
    }

    public void Initialize()
    {
        m_villagerGenerator = new VillagerGenerator();
        m_villagerGenerator.Initialize();

        m_population = new List<VillagerData>();
        OnPopulationChanged?.Invoke(m_population);
        m_villagerQueue = new List<VillagerData>();

#if UNITY_EDITOR
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action<int, int, int>>("initpop", (int child, int adults, int elders) =>
        {
            CreateRandomVillagers(child,adults,elders);
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

    #region Villager Handling Utilities

    public void IncreaseFatigue(VillagerData data, int value)
    {
        data.IncreaseFatigue(value);
    }

    public void DecreaseFatigue(VillagerData data, int value)
    {
        data.DecreaseFatigue(value);
    }

    public void ApplyHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.ApplyHealthStatus(status);
    }

    public void RemoveHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.RemoveHealthStatus(status);
    }

    public void GetPregnant()
    {
        bool _NoPregnancy = false;
        foreach (VillagerData villager in m_population)
        {
            if (villager.IsAdult() && villager.GetGender() == VillagerData.Gender.FEMALE)
            {
                villager.Impregnate();
                Debug.Log($"is pregnant : {villager}");
                _NoPregnancy = true;
                break;
            }
            
        }
        if (_NoPregnancy == false)
        {
            Debug.Log("nobody is pregnant");
            
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

    public void GetSick()
    {
        var rng = GameManager.RNG;
        int randomNumber = rng.Next(0, m_population.Count);
        m_population[randomNumber].ApplyHealthStatus(VillagerData.HealthStatus.SICK);
        Debug.Log(m_population[randomNumber]);

        Debug.Log("is sick, yeaaah");
    }
    #endregion

    #region Villager Setup Utilities

    private void CreateRandomVillager(int agetoset)
    {
        var name = m_villagerGenerator.GenerateName();
        var age = m_villagerGenerator.GetAgeDependingOnAgeStage(agetoset);
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

    private void CreateRandomVillagers( int kids, int adults, int elders)
    {
         while (kids > 0)
            {
                CreateRandomVillager(1);
                AddVillagerToPopulation();
                kids--;
            }
        while (adults > 0)
            {
                CreateRandomVillager(2);
                AddVillagerToPopulation();
                adults--;
            }
        while (elders > 0)
            {
                CreateRandomVillager(3);
                AddVillagerToPopulation();
                elders--;
            }
        
    }

    private void CreateRandomVillagersInQueue(int amount)
    {
        while (amount > 0)
        {
            CreateRandomVillager(m_villagerGenerator.SetAge());
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
