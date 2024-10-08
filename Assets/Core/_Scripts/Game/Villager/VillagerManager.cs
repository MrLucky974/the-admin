using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    ResourceSystem m_resourceSystem;
    [SerializeField] VillagerDataDisplay m_villagerDataDisplay;

    private void Start()
    {
        m_resourceSystem = GetComponent<ResourceSystem>();
        m_villagerDataDisplay = GetComponent<VillagerDataDisplay>();

        var commandLog = GameManager.Instance.GetCommandLog();
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("checkup", (string identifier) =>
        {
            var population = m_resourceSystem.GetPopulation();
            foreach (VillagerData villager in population)
            {
                if (identifier == villager.GetID())
                {

                    m_villagerDataDisplay.Display(villager);
                    Debug.Log("villager set");
                    break;
                }
            }

        }));
    }
    
    public void IncreaseFatigue(int toIncrease, VillagerData villager)
    {
        villager.IncreaseFatigue(toIncrease);
        Debug.Log(villager.GetFatigue());
    }
    
    public void DecreaseFatigue(int toDecrease, VillagerData villager)
    {
        villager.DecreaseFatigue(toDecrease);
    }
    
    public void GetPregnant()
    {
        var population = m_resourceSystem.GetPopulation();
        foreach (VillagerData villager in population)
        {
            if (villager.IsAdult() && villager.GetGender() == VillagerData.Gender.FEMALE)
            {
                villager.Impregnate();
            }
        }
    }

    public void ApplyHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.AddHealthStatus(status);
    }

    public void RemoveHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.RemoveHealthStatus(status);
    }
}
