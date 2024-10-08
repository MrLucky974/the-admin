using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    private VillagerGenerator m_villagerGenerator = new VillagerGenerator();
    private List<VillagerData> m_population;
    private VillagerData m_currentVillager;
    private List<VillagerData> m_villagerQueue;

    public ReadOnlyCollection<VillagerData> GetPopulation()
    {
        return m_population.AsReadOnly();
    }

    private void Start()
    {
        m_population = new List<VillagerData>();
        m_villagerQueue = new List<VillagerData>();
        InitPopulation(3);
        ListPopulation();
    }
    
    void AddVillagerToPopulation()
    {
        if (m_population == null)
        {
            m_population = new List<VillagerData>();
        }
        m_population.Add(m_currentVillager);
        AssignVillagerID();
    }

    void CreateVillager()
    {
        var name = m_villagerGenerator.GenerateName();
        var age = m_villagerGenerator.SetAge();
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

    void InitPopulation(int numberToCreate)
    {
        while (numberToCreate > 0)
        {
            CreateVillager();
            AddVillagerToPopulation();
            numberToCreate--;
        }
    }

    void CreateWaitingVillagers(int numberToCreate)
    {
        while (numberToCreate > 0)
        {
            CreateVillager();
            m_villagerQueue.Add(m_currentVillager);
            Debug.Log(m_villagerQueue.Count);
            numberToCreate--;
        }
    }
    void AddWaitingVillagerToPopulation(int villagerToAdd)
    {
        VillagerData visitor = m_villagerQueue[villagerToAdd];
        
        m_villagerQueue.Remove(visitor);

        m_currentVillager = visitor;
        m_population.Add(visitor);
        
        AssignVillagerID();
    }

    void KillVillagers(int numberToKill)
    {
        if (numberToKill <= m_population.Count)
        {
            Debug.Log($"is getting killed {m_population[numberToKill]})");
            m_population.Remove(m_population[numberToKill]);
        }
    }
    
    void AssignVillagerID()
    {
        var name = m_currentVillager.GetName();
        var index = m_population.Count - 1;
        m_currentVillager.SetID(m_villagerGenerator.GenerateID(name, index));
    }
    
    //DEBUG
    void ListPopulation()
    {
        foreach (var villager in m_population)
        {
            Debug.Log($"Population is composed by : {villager}");
        }
    }
}
