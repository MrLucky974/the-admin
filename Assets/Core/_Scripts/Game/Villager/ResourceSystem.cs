using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceSystem : MonoBehaviour
{
    [SerializeField] VillagerManager villagerManager;
    [SerializeField] VillagerGenerator villagerGenerator = new VillagerGenerator();
    public List<VillagerData> population;
    VillagerData currentVillager;
    List<VillagerData> waitingVillagers;

    private void Start()
    {
        population = new List<VillagerData>();
        waitingVillagers = new List<VillagerData>();
        InitPopulation(3);
        ListingPopulation();



    }
    void AddVillagerToPopulation()
    {
        if (population == null)
        {
            population = new List<VillagerData>();
        }
        population.Add(currentVillager);
        GiveVillagerAnID();
    }

    void CreateVillager()
    {
        VillagerData villager = new VillagerData();
        villager.UpdateName(villagerGenerator.GetName());
        villager.NewAge(villagerGenerator.SetAge());
        villager.UpdateAgeStatus();
        villager.SetGender(villagerGenerator.SetGender());
        villager.SetPersonality((VillagerData.Personality)villagerGenerator.SetPersonality());
        currentVillager = villager;
        Debug.Log($"villager created : {currentVillager}");
    }

    void InitPopulation(int numberToCreate)
    {
        while (numberToCreate > 0)
        {
            CreateVillager();
            AddVillagerToPopulation();
            GiveVillagerAnID();
            numberToCreate--;

        }
        
    }

    void CreateWaitingVillagers(int numberToCreate)
    {
        while (numberToCreate > 0)
        {
            CreateVillager();
            waitingVillagers.Add(currentVillager);
            Debug.Log(waitingVillagers.Count);
            numberToCreate--;
        }


    }
    void AddWaitingVillagerToPopulation(int villagerToAdd)
    {
        VillagerData newCommer = waitingVillagers[villagerToAdd];
        waitingVillagers.Remove(waitingVillagers[villagerToAdd]);
        population.Add(newCommer);
        GiveVillagerAnID();
    }

    void KillVillagers(int numberToKill)
    {
        if (numberToKill <= population.Count)
        {
            Debug.Log($"is getting killed {population[numberToKill]})");
            population.Remove(population[numberToKill]);
        }
    }
    void GiveVillagerAnID()
    {
        currentVillager.SetID(villagerGenerator.SetVillagerID(currentVillager.GetName(), population.Count - 1));
    }
    //DEBUG

    void ListingPopulation()
    {
        int currentlyListed = 0;
        foreach (var villager in population)
        {
            Debug.Log($"Population is composed by : { population[currentlyListed]}");
            currentlyListed++;
        }
    }
}
