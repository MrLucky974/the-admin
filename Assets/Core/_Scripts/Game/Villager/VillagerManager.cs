using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    ResourceSystem resourceSystem;
    [SerializeField] VillagerDataDisplay villagerDataDisplay;

    private void Start()
    {
        resourceSystem = GetComponent<ResourceSystem>();
        villagerDataDisplay = GetComponent<VillagerDataDisplay>();
        

        var commandLog = GameManager.Instance.GetCommandLog();
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("checkup", (string identifier) =>
        {
            foreach (VillagerData villager in resourceSystem.population)
            {
                if (identifier == villager.GetID())
                {

                    villagerDataDisplay.Display(villager);
                    Debug.Log("villager set");
                    break;
                }
            }

        }));
    }
    public void increaseFatigue(int toIncrease, VillagerData villager)
    {
        villager.IncreaseFatigue(toIncrease);
        Debug.Log(villager.GetFatigue());
    }
    public void decreaseFatigue(int toDecrease, VillagerData villager)
    {
        villager.DecreaseFatigue(toDecrease);
    }
    public void GetPregnant()
    {
        foreach (VillagerData villager in resourceSystem.population)
        {
            if (villager.IsAdult() && villager.GetGender() == VillagerData.Genre.FEMALE)
            {
                ApplyHealthStatus(villager, VillagerData.HealthStatus.PREGNANT);
            }
        }

    }

    public void ApplyHealthStatus(VillagerData data, VillagerData.HealthStatus status)
    {
        data.AddHealthStatus(status);
    }
}
