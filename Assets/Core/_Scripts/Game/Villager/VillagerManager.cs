using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using static VillagerData;

// TODO : Replace some OnPopulationChanged calls by a more individual focused event
public class VillagerManager : MonoBehaviour
{
    public event Action<List<VillagerData>> OnPopulationChanged;

    private VillagerGenerator m_villagerGenerator;

    private List<VillagerData> m_population;
    private VillagerData m_currentVillager;
    private List<VillagerData> m_villagerQueue;

    private TimeManager m_timeManager;
    private CommandLogManager m_commandLog;

    public ReadOnlyCollection<VillagerData> GetPopulation()
    {
        return m_population.AsReadOnly();
    }

    private void OnDestroy()
    {
        m_timeManager.OnWeekEnded -= OnNewWeek;
        m_timeManager.OnDayEnded -= OnNewDay;
    }

    public void Initialize()
    {
        m_villagerGenerator = new VillagerGenerator();
        m_villagerGenerator.Initialize();

        m_population = new List<VillagerData>();
        OnPopulationChanged?.Invoke(m_population);
        m_villagerQueue = new List<VillagerData>();

        m_timeManager = GameManager.Instance.GetTimeManager();
        m_timeManager.OnWeekEnded += OnNewWeek;
        m_timeManager.OnDayEnded += OnNewDay;

        m_commandLog = GameManager.Instance.GetCommandLog();
        var commandSystem = GameManager.Instance.GetCommands();

#if UNITY_EDITOR
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

        commandSystem.AddCommand(new CommandDefinition<Action>("showid", () =>
        {
            ShowIDs();
        }));
#endif
    }

    #region Villager Gameplay Handlers

    public void OnNewWeek(int week)
    {
        FeedPopulation();
        GetPregnant();
    }

    public void OnNewDay(int day)
    {
        AgePopulation();
        UpdatePregnantWomenStatus();
    }

    public void AgePopulation()
    {
        foreach (VillagerData villager in m_population)
        {
            villager.GetOlder();
            //Debug.Log($"{villager.GetName()} got older : {villager.GetAgeStage()}");
        }
        OnPopulationChanged?.Invoke(m_population);
    }

    public void FeedPopulation()
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

    public void GetPregnant()
    {
        // Get a list of all adult women inside the base not already pregnant
        var women = m_population.FindAll((villager) => villager.IsAdult()
            && villager.IsMale() is false
            && villager.IsOnExpedition() is false
            && villager.IsPregnant() is false
        );

        // Check if there is at least one male on the base
        bool hasMale = m_population.Any((villager) => villager.IsAdult()
            && villager.IsMale()
            && villager.IsOnExpedition() is false
        );

        if (women.Count < 1 || !hasMale)
        {
            Debug.Log("nobody can be made pregnant");
            return;
        }

        // Give a random number of women that are going to get pregnant on this game tick
        var rng = GameManager.RNG;
        const int MINIMUM_PREGNANCY_COUNT = 1; // Can be one since we already check that there's at least one women on base
        const int PREGNANCY_CHANCE = 20;
        int maxPregnancyCount = Mathf.Max(MINIMUM_PREGNANCY_COUNT, women.Count > (100 / PREGNANCY_CHANCE) + 1 ? (women.Count * PREGNANCY_CHANCE) / 100 : women.Count);
        int expectedPregnancyCount = rng.Next(MINIMUM_PREGNANCY_COUNT, maxPregnancyCount + 1); //+1 is added to max since Next does [min, max)

        Debug.Log($"{women.Count} available for mating");
        Debug.Log($"fucking {expectedPregnancyCount} people ({MINIMUM_PREGNANCY_COUNT} | {maxPregnancyCount})");

        Queue<VillagerData> futurePregnancies = new Queue<VillagerData>();
        for (int i = 0; i < expectedPregnancyCount; i++)
        {
            var value = women.PickRandom();
            women.Remove(value);
            futurePregnancies.Enqueue(value);
        }

        while (futurePregnancies.TryDequeue(out var villager))
        {
            villager.Impregnate(GetRandomMale());
            Debug.Log($"is pregnant: {villager}");
        }

        OnPopulationChanged?.Invoke(m_population);
    }

    public void UpdatePregnantWomenStatus()
    {
        var pregnantIndividuals = m_population.FindAll((villager) => villager.IsPregnant());
        foreach (VillagerData villager in pregnantIndividuals)
        {
            villager.UpdatePregnancy();
            if (villager.GetPregnancyDuration() >= 3)
            {
                villager.RemoveHealthStatus(HealthStatus.PREGNANT);
                var baby = CreateBaby(villager, villager.GetMate());
                AddVillagerToPopulation();
                m_commandLog.AddLog($"villager: {baby.GetName()} is born!", GameManager.ORANGE);
            }
        }
    }

    #endregion

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

    private VillagerData GetRandomMale()
    {
        // Select all adult males that are on the base
        var males = m_population.FindAll((villager) => villager.IsAdult()
            && villager.IsMale()
            && villager.IsOnExpedition() is false
        );
        var rng = GameManager.RNG;
        return males.PickRandom(rng);
    }

    public void SetWorkingStatus(VillagerData data, WorkingStatus status)
    {
        data.SetWorkingStatus(status);
        OnPopulationChanged?.Invoke(m_population);
    }

    public void GetSick()
    {
        var rng = GameManager.RNG;
        int randomNumber = rng.Next(0, m_population.Count);
        m_population[randomNumber].ApplyHealthStatus(HealthStatus.SICK);
        OnPopulationChanged?.Invoke(m_population);
        Debug.Log(m_population[randomNumber]);
        Debug.Log("is sick, yeaaah");
    }

    #endregion

    #region Villager Setup Utilities

    private void CreateRandomVillager(AgeStage stage)
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

        SetVillager(villager);
    }

    private VillagerData CreateBaby(VillagerData parent1, VillagerData parent2)
    {
        var name = m_villagerGenerator.GenerateNameFromParents(parent1, parent2);
        var gender = m_villagerGenerator.SelectRandomGender();
        var personality = m_villagerGenerator.SelectRandomPersonality();

        var villager = new VillagerData.Builder(name)
        .SetAge(AGE_RANGE[AgeStage.KID].min)
            .SetGender(gender)
            .SetPersonality(personality)
            .Build();

        SetVillager(villager);
        return villager;
    }

    private void SetVillager(VillagerData data)
    {
        m_currentVillager = data;
        Debug.Log($"set current villager: {m_currentVillager}");
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

#if UNITY_EDITOR
    private void ListPopulation()
    {
        m_population.Print();
    }
#endif

    #endregion
}
