using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    ResourceSystem resourceSystem;

    private void Start()
    {
        resourceSystem = GetComponent<ResourceSystem>();
    }
    public void decreaseFatigue(int toDecrease)
    {

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
