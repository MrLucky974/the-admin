using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
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
        m_villagerQueue = new List<VillagerData>();

#if UNITY_EDITOR
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action>("initpop", () =>
        {
            CreateRandomVillagers(3);
            ListPopulation();
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
        foreach (VillagerData villager in m_population)
        {
            if (villager.IsAdult() && villager.GetGender() == VillagerData.Gender.FEMALE)
            {
                villager.Impregnate();
            }
        }
    }

    public void SendVillagerToRoom(string villagerId,string roomId)
    {
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        RoomData room = roomManager.GetRoomWithId(roomId);
        VillagerData villager =  GetVillagerByID(villagerId);

        if (villager == null) return; // Check villager != null
        if (villager.GetID() == villagerId)
        {
            if (room != null)
            {
                roomManager.TryToRepairRoom(villager,roomId,5); // Check if can repair the room
                Debug.Log(room.GetVillagersInRoom().Count);
            }
        }
    }

    #endregion

    #region Villager Setup Utilities

    private void CreateRandomVillager()
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
        m_currentVillager = null;
    }

    private void CreateRandomVillagers(int amount)
    {
        while (amount > 0)
        {
            CreateRandomVillager();
            AddVillagerToPopulation();
            amount--;
        }
    }

    private void CreateRandomVillagersInQueue(int amount)
    {
        while (amount > 0)
        {
            CreateRandomVillager();
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
