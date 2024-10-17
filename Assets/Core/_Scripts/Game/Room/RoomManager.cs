using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private NarratorSystem m_narrator;

    ResourceHandler m_resourceHandler;
    ReputationHandler m_reputationHandler;
    RoomData[] m_roomArray;

    ArrayList m_ids = new ArrayList();
    ArrayList m_roomNames = new ArrayList();

    public List<string> ACCIDENT_LIST;

    Coroutine m_degradeCoroutine;
    Coroutine m_ressCoroutine;

    public const int DEGRADATION = 4;
    public const float TIMEDEGRADE = 1f;
    public const int DESTROY_REPUTATION_COST = 15;

    public const int DEFAULT_FATIGUE_COST = 2;
    public const int MAX_CHANCE_TOBE_DESTROY = 30;

    public const int BONUS_FATIGUE_COST = 1;
    public const int MALUS_FATIGUE_COST = 2;

    public const int BONUS_REPAIR_SPEED = 1;
    public const int MALUS_REPAIR_SPEED = 2;
    float m_repairTimeBonus = 0;

    GameManager m_gm;
    TimeManager m_timeManager;

    public void InitAccidentList()
    {
        ACCIDENT_LIST.Add("there was a fire");
        ACCIDENT_LIST.Add("there was a flood");
        ACCIDENT_LIST.Add("there was a collapse");
        ACCIDENT_LIST.Add("there was an explosion");
    }

    public void Initialize()
    {
        m_gm = FindObjectOfType<GameManager>();
        m_roomArray = FindObjectsOfType<RoomData>();
        m_resourceHandler = FindObjectOfType<ResourceHandler>();
        m_reputationHandler = FindObjectOfType<ReputationHandler>();
        m_timeManager = GameManager.Instance.GetTimeManager();
        InitRoom();
        m_narrator = GameManager.Instance.GetNarrator();
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
        m_ressCoroutine = StartCoroutine(GenerateRessources());
        m_timeManager.OnDayEnded += RandomDamagedRoomEvent;

        //--- Events
        m_narrator.Subscribe<DamagedRoomEvent>(RoomEvents.DAMAGED_ROOM, OnRoomDamaged);

        //---
        if (m_roomArray.Length == 0)
        {
            Debug.LogError("No room in this scene", this.gameObject);
        }
        InitAccidentList();
        InitCommands();
    }

    void InitRoom()
    {
        InitIds(m_roomArray.Length - 1);
        foreach (RoomData room in m_roomArray)
        {
            room.OnRoomDestroyed += RoomWasDestroyed;
        }
    }

    void InitIds(int roomNum)
    {
        for (int i = 0; i <= roomNum; i++)
        {
            m_ids.Add(i + 1);
        }
        InitializeRoomsId();
    }

    void InitializeRoomsId()
    {
        var rng = GameManager.RNG;
        foreach (RoomData room in m_roomArray)
        {
            int index = rng.Next(0, m_ids.Count);
            room.roomId = "R" + m_ids[index];
            m_ids.RemoveAt(index);
            Debug.Log(room.name + " / " + room.roomId);
        }
    }

    #region Commandes
    void InitCommands()
    {
        m_gm.GetCommands().AddCommand(new CommandDefinition<Action<string, string>>("repair", "Sends a villager to repair a room", (string roomId, string villagerId) =>
        {
            m_gm.GetVillagerManager().SendVillagerRepairRoom(villagerId, roomId);
        }));
        m_gm.GetCommands().AddCommand(new CommandDefinition<Action<string>>("upgrade", "Upgrades a room which can give resources or more durability", (string roomId) =>
        {
            UpgradeRoom(roomId);
        }));
#if UNITY_EDITOR
        m_gm.GetCommands().AddCommand(new CommandDefinition<Action<string, int>>("damage", "Apply damage to a room", (string roomId, int damage) =>
        {
            ApplyDamageToRoomWithID(roomId, damage);
        }));
#endif
    }
    #endregion

    #region Room Handling Utilities

    public void RoomWasDestroyed(RoomData roomDestroyed)
    {
        if (roomDestroyed is VillagerUpgradeRoomData) //for villager upgrades
        {
            VillagerUpgradeRoomData room = roomDestroyed.GetComponent<VillagerUpgradeRoomData>();
            CancelVillRoomUpgrade(room);
        }
        m_reputationHandler.DecreaseReputation(DESTROY_REPUTATION_COST);

    }

    public void ApplyDamageToRoomType(RoomType roomType, int damage)
    {
        RoomData room = GetRoomOfType(roomType);
        room.IncrementDurability(-damage);
    }

    public void ApplyDamageToRoomWithID(String id, int damage)
    {
        RoomData room = GetRoomWithId(id);
        room.IncrementDurability(-damage);
    }

    public void StartRepairRoom(string roomId, int scrapsCost, VillagerData vill)
    {
        RoomData currentRoom = GetRoomWithId(roomId);
        foreach (RoomData room in m_roomArray)
        {
            if (room.roomId == roomId)
            {
                ArrayList villagers = currentRoom.GetVillagersInRoom();

                float repairSpeed = ComputRepairSpeed(vill) - m_repairTimeBonus;  //
                if (currentRoom.roomState == RoomData.RoomState.DESTROYED)
                {
                    repairSpeed = repairSpeed * 2;
                    scrapsCost = scrapsCost * 2;
                }
                VillagerManager vm = m_gm.GetVillagerManager();
                foreach (VillagerData villager in room.GetVillagersInRoom())
                {
                    vm.SetWorkingStatus(villager, VillagerData.WorkingStatus.MAINTENANCE); //change working status of villager in room
                }
                StartCoroutine(RepairRoomCoroutine(currentRoom, repairSpeed));
                m_resourceHandler.ConsumeScraps(scrapsCost);
            }
        }
    }

    public void RepairRoomComplete(RoomData room)
    {
        VillagerManager vm = m_gm.GetVillagerManager();
        foreach (VillagerData villager in room.GetVillagersInRoom())
        {
            //Increase fatigue to villager in room
            if (IsInjured(villager))
            {  // compute injured chance
                vm.ApplyHealthStatus(villager, VillagerData.HealthStatus.INJURED);
            }
            vm.IncreaseFatigue(villager, ComputeRepairFatigueCost(villager));
            vm.SetWorkingStatus(villager, VillagerData.WorkingStatus.IDLE);
            m_gm.GetCommandLog().AddLog($"{room.roomId} repaired", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }
        room.RepairRoom();
    }

    public int ComputRepairSpeed(VillagerData villager)
    {
        int repairSpeed = VillagerData.DEFAULT_WORKING_SPEED;
        switch (villager.GetPersonality())
        {
            case VillagerData.Personality.HARDWORKER:
                repairSpeed = VillagerData.DEFAULT_WORKING_SPEED - BONUS_REPAIR_SPEED;
                break;
            case VillagerData.Personality.LAZY:
                repairSpeed = VillagerData.DEFAULT_WORKING_SPEED + MALUS_REPAIR_SPEED;
                break;
        }
        Debug.Log("REPARATION " + repairSpeed);
        return repairSpeed;
    }

    public bool IsInjured(VillagerData villager)
    {
        int factor = villager.GetFatigue();
        int chance = GameManager.RNG.Next(0, VillagerData.MAX_FATIGUE);
        if (chance < factor)
        {
            villager.ApplyHealthStatus(VillagerData.HealthStatus.INJURED);
            Debug.Log("AIIIE " + factor + " - " + chance);
            return true;
        }
        Debug.Log("TRANQUILLE " + factor + " - " + chance);
        return false;
    }

    public int ComputeRepairFatigueCost(VillagerData villager)
    {
        int cost = DEFAULT_FATIGUE_COST;
        if (villager.GetHealthStatus() == VillagerData.HealthStatus.SICK ||
           villager.GetHealthStatus() == VillagerData.HealthStatus.INJURED ||
           villager.GetHealthStatus() == VillagerData.HealthStatus.PREGNANT ||
           villager.GetHealthStatus() == VillagerData.HealthStatus.STARVED ||
           villager.GetPersonality() == VillagerData.Personality.HARDWORKER ||
           villager.GetAgeStage() == VillagerData.AgeStage.ELDER
           )
        {
            cost = DEFAULT_FATIGUE_COST + MALUS_FATIGUE_COST;
            Debug.Log("MECHANT " + cost);
        }
        if (villager.GetPersonality() == VillagerData.Personality.LAZY
         )
        {
            cost = DEFAULT_FATIGUE_COST - BONUS_FATIGUE_COST;
            Debug.Log("FLEMME" + cost);
        }
        Debug.Log("GENTIL " + cost);
        return DEFAULT_FATIGUE_COST;
    }

    public void TryToRepairRoom(VillagerData villager, string roomId)
    {
        var commandLog = m_gm.GetCommandLog();
        var room = GetRoomWithId(roomId);
        int scrapsCost = room.GetRepairCost();

        if (villager.GetAgeStage() == VillagerData.AgeStage.KID)
        {
            commandLog.AddLogError($"repair: cannot repair room {roomId} with {villager.GetID()}, because it's a kid!");
            return;
        }

        if (room.GetVillagersInRoom().Count != 0)
        {
            commandLog.AddLogError($"repair: cannot repair room {roomId}, someone is already fixing this room!");
            return;
        }

        if (room.roomState == RoomData.RoomState.DESTROYED)
        {
            scrapsCost = scrapsCost * 2;
        }

        if (villager.GetWorkingStatus() != VillagerData.WorkingStatus.IDLE)
        {
            commandLog.AddLogError($"repair: villager {villager.GetID()} is occupied!");
            return;
        }

        if (!m_resourceHandler.HasEnoughResources(0, 0, scrapsCost)) // --Check for ressources
        {
            commandLog.AddLogError($"repair: cannot repair room {roomId}, not enough resources (needs {scrapsCost} SCRAPS)!");
            return;
        }

        if (room.roomState != RoomData.RoomState.FUNCTIONAL) // --Check if room can be repaired 
        {
            room.AddVillagerInRoom(villager); // --Add villager to room
            StartRepairRoom(roomId, scrapsCost, villager);
        }
        else
        {
            commandLog.AddLogError($"repair: room {roomId} is functionnal!");
            return;
        }
    }

    public void AddVillagerToRoom(VillagerData villager, string roomId)
    {
        GetRoomWithId(roomId).AddVillagerInRoom(villager);
    }

    #region Upgrade funcs

    public void UpgradeRoom(string roomId)
    {
        UpgradableRoomData[] ressRooms = FindObjectsOfType<UpgradableRoomData>();
        foreach (UpgradableRoomData room in ressRooms)
        {
            //TODO create a function try to upgrade 
            if (room.roomId == roomId)
            {
                if (m_resourceHandler.HasEnoughResources(0, 0, room.upgradeCost))
                {
                    if (room.roomState == RoomData.RoomState.DESTROYED)
                    {
                        m_gm.GetCommandLog().AddLogError($"upgrade {room.roomId} failed this room is destroyed");
                        return;
                    }
                    m_resourceHandler.ConsumeScraps(room.upgradeCost);
                    UpgradeRoom(room); //Upgrade 
                    m_gm.GetCommandLog().AddLog($"{roomId} upgraded", GameManager.ORANGE);
                    return;
                }
                else
                {
                    m_gm.GetCommandLog().AddLogError($"upgrade {room.roomId} failed not enough resources");
                    return;
                }
            }
            else
            {
                if (roomId == "")
                {
                    m_gm.GetCommandLog().AddLogError($"specify room id example: upgrade R1");
                    return;
                }
            }
        }
        m_gm.GetCommandLog().AddLogError($"upgrade {roomId} failed you cant upgrade this room");
        return;
    }

    public void UpgradeRoom(UpgradableRoomData room)
    {
        room.Upgrade();
        if (room is VillagerUpgradeRoomData)
        {
            VillagerUpgradeRoomData upRoom = room.GetComponent<VillagerUpgradeRoomData>();
            UpgradeVillRoom(upRoom);

        }
    }

    public void UpgradeVillRoom(VillagerUpgradeRoomData room)
    {
        switch (room.UpgradeType)
        {
            case VillagerUpgradeRoomData.UpgradeTypes.REPAIR_SPEED:
                UpgradeRepairSpeed(room);
                break;
            case VillagerUpgradeRoomData.UpgradeTypes.FATIGUE_REGEN:
                UpgradeFatigueRegen(room);
                break;
        }
    }

    public void UpgradeRepairSpeed(VillagerUpgradeRoomData room)
    {
        IncreaseGlobalRepairSpeed(room.BonusValue);
        m_gm.GetCommandLog().AddLog($"reparation time is faster", GameManager.ORANGE);
    }

    public void UpgradeFatigueRegen(VillagerUpgradeRoomData room)
    {
        VillagerManager vm = FindObjectOfType<VillagerManager>();
        ReadOnlyCollection<VillagerData> population = vm.GetPopulation();
        foreach (VillagerData villager in population)
        {
            vm.IncreaseRecoveryValue(villager, room.BonusValue);
            Debug.Log(villager.GetName() + "-" + villager.GetRecoveryValue());
        }
        m_gm.GetCommandLog().AddLog($"the population rests faster ", GameManager.ORANGE);
    }

    public void CancelVillRoomUpgrade(VillagerUpgradeRoomData room)
    {
        switch (room.UpgradeType)
        {
            case VillagerUpgradeRoomData.UpgradeTypes.REPAIR_SPEED:
                m_repairTimeBonus = 0;
                break;
            case VillagerUpgradeRoomData.UpgradeTypes.FATIGUE_REGEN:
                VillagerManager vm = FindObjectOfType<VillagerManager>();
                ReadOnlyCollection<VillagerData> population = vm.GetPopulation();
                foreach (VillagerData villager in population)
                {
                    vm.DecreaseRecoveryValue(villager, room.BonusValue);
                    Debug.Log(villager.GetName() + "-" + villager.GetRecoveryValue());
                }
                break;
        }

    }

    public void IncreaseGlobalRepairSpeed(float value)
    {
        m_repairTimeBonus += value;
    }

    #endregion

    public RoomData PickRandomRoom()
    {
        int rng = GameManager.RNG.Next(0, m_roomArray.Length);
        RoomData room = m_roomArray[rng];
        return room;
    }

    public RoomData GetRoomWithId(string roomId)
    {
        foreach (RoomData room in m_roomArray)
        {
            if (room.roomId == roomId) { return room; }
        }
        return null;
    }

    public RoomData GetRoomOfType(RoomType type)
    {
        RoomData findedRoom = null;
        foreach (RoomData room in m_roomArray)
        {
            if (room.roomType == type)
            {
                findedRoom = room;
            }
        }
        return findedRoom;
    }

    RoomData GetRoomInRoomArray(RoomData roomInst)
    {
        foreach (RoomData room in m_roomArray)
        {
            if (room == roomInst) { return room; }
        }
        return null;
    }

    public ArrayList GetVillagerInRoom(RoomData room)
    {
        return room.GetVillagersInRoom();
    }
    #endregion

    #region Coroutines


    #region Events
    public void OnRoomDamaged(DamagedRoomEvent data)
    {
        RoomData room = GetRoomOfType(data.roomType);
        ApplyDamageToRoomWithID(room.roomId, data.damage);
        string accident = ACCIDENT_LIST[GameManager.RNG.Next(0, ACCIDENT_LIST.Count)];
        m_gm.GetCommandLog().AddLog($"{accident} that damaged {GetRoomOfType(data.roomType).roomId}", GameManager.ORANGE);
    }

    public void RandomDamagedRoomEvent(int day)
    {
        int rng = GameManager.RNG.Next(0, MAX_CHANCE_TOBE_DESTROY);
        if (rng < 10)
        {
            RoomData room = PickRandomRoom();
            var data = new DamagedRoomEvent
            {
                roomType = room.roomType,
                damage = 51

            };
            m_narrator.TriggerEvent<DamagedRoomEvent>(RoomEvents.DAMAGED_ROOM, data);
        }
    }
    #endregion

    IEnumerator DegradeRoom()
    {
        RoomData room = PickRandomRoom();
        for (int i = 0; i < m_roomArray.Length; i++)
        {
            room = PickRandomRoom();
            yield return new WaitForSeconds(1);
            if (room.roomState != RoomData.RoomState.DESTROYED)
            {
                room.IncrementDurability(-DEGRADATION);
            }
        }

        yield return new WaitForSeconds(TimeManager.DAY_IN_SECONDS / m_roomArray.Length);
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
    }
    IEnumerator GenerateRessources()
    {
        ResourceUpgradeRoomData[] ressRooms = FindObjectsOfType<ResourceUpgradeRoomData>();
        foreach (ResourceUpgradeRoomData room in ressRooms)
        {
            if (room.upgradeState == ResourceUpgradeRoomData.UpgradeState.UPGRADED) // check if the room is upgraded
            {
                switch (room.ressourceType) // check 
                {
                    case ResourceType.RATIONS: // ration
                        m_resourceHandler.AddRations(room.ressValue);
                        break;
                    case ResourceType.MEDS: // meds
                        m_resourceHandler.AddMeds(room.ressValue);
                        break;
                }
            }
            yield return new WaitForSeconds(room.genRessTime);
        }
        yield return new WaitForSeconds(1);
        m_ressCoroutine = StartCoroutine(GenerateRessources());
    }

    IEnumerator RepairRoomCoroutine(RoomData roomToRepair, float time)
    {
        roomToRepair.SetRoomState(RoomData.RoomState.REPAIRING);
        yield return new WaitForSeconds(time);
        RepairRoomComplete(roomToRepair);
    }
    #endregion

}


