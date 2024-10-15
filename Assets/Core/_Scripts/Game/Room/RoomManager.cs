
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.HDROutputUtils;
using System;
using static UpVillRoomData;
using System.Collections.ObjectModel;


public class RoomManager : MonoBehaviour
{

    Test m_admin;


    ResourceHandler m_resourceHandler;
    ReputationHandler m_reputationHandler;
    RoomData[] m_roomArray;


    ArrayList m_ids = new ArrayList();
    ArrayList m_roomNames = new ArrayList();


    Coroutine m_degradeCoroutine;
    Coroutine m_ressCoroutine;

    public const int DEGRADATION = 4;
    public const float TIMEDEGRADE = 1f;

    float m_repairTimeBonus = 0;

    GameManager m_gm;

  

    public void Initialize()
    {
        m_gm = FindObjectOfType<GameManager>();
        m_roomArray = FindObjectsOfType<RoomData>();
        m_resourceHandler = FindObjectOfType<ResourceHandler>();
        m_reputationHandler = FindObjectOfType<ReputationHandler>();
        InitRoom();
        m_admin = FindObjectOfType<Test>();
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
        m_ressCoroutine = StartCoroutine(GenerateRessources());

        if (m_roomArray.Length == 0){
            Debug.LogError("No room in this scene", this.gameObject);
        }

        if (!m_resourceHandler)
        {
            Debug.LogError("resource handler not find", this.gameObject);
        }

        if (!m_reputationHandler)
        {
            Debug.LogError("reputation handler not find", this.gameObject);
        }
        InitCommands();
    }

    void InitRoom()
    {
        InitIds(m_roomArray.Length - 1);
        foreach (RoomData room in m_roomArray)
        {
            room.OnRoomDestroyed +=RoomWasDestroyed;
        }
    }

    void InitIds(int roomNum)
    {
        for (int i = 0; i <= roomNum; i++)
        {
            m_ids.Add(i+1);
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
            Debug.Log(room.name+" / "+room.roomId);
        }
    }

    #region Commandes
    void InitCommands()
    {
        m_gm.GetCommands().AddCommand(new CommandDefinition<Action<String, String>>("repair", (String roomId, String villagerId) =>
        {
            m_gm.GetVillagerManager().SendVillagerRepairRoom(villagerId, roomId);
        }));
        m_gm.GetCommands().AddCommand(new CommandDefinition<Action<String>>("upgrade", (String roomId) =>
        {
            UpgradeRoom(roomId);
        }));
    }
    #endregion

    #region Room Handling Utilities

    public void RoomWasDestroyed(RoomData roomDestroyed)
    {
        if(roomDestroyed is UpVillRoomData) //for villager upgrades
        { 
            UpVillRoomData room = roomDestroyed.GetComponent<UpVillRoomData>();
            CancelVillRoomUpgrade(room);
        }
        m_reputationHandler.DecreaseReputation(15); //TODO add a reputation value
       
    }

    public void ApplyDamageToRoomType(RoomType roomType,int damage)
    {
        RoomData room = GetRoomOfType(roomType);
        room.IncrementDurability(-damage);
    }

    public void StartRepairRoom(string roomId, int scrapsCost)
    {
        RoomData currentRoom = GetRoomWithId(roomId);
        foreach (RoomData room in m_roomArray)
        {
            if (room.roomId == roomId)
            {
                ArrayList villagers = currentRoom.GetVillagersInRoom();
                float repairSpeed = VillagerData.DEFAULT_WORKING_SPEED-m_repairTimeBonus;  //
                if(currentRoom.roomState == RoomData.RoomState.DESTROYED)
                {
                    repairSpeed = repairSpeed * 2;
                    scrapsCost = scrapsCost * 2;
                }
                VillagerManager vm = m_gm.GetVillagerManager();
                foreach (VillagerData villager in room.GetVillagersInRoom())
                {
                    vm.SetWorkingStatus(villager, VillagerData.WorkingStatus.MAINTENANCE); //change working status of villager in room
                }
                StartCoroutine(RepairRoomCoroutine(currentRoom,repairSpeed));
                m_resourceHandler.ConsumeScraps(scrapsCost);
            }
        }
    }

    public void RepairRoomComplete(RoomData room)
    {
        VillagerManager vm = m_gm.GetVillagerManager();
        foreach (VillagerData villager in room.GetVillagersInRoom()) 
        {
            vm.IncreaseFatigue(villager,5); //Increase fatigue to villager in room //TODO compute fatigue value
            vm.SetWorkingStatus(villager, VillagerData.WorkingStatus.IDLE);
            m_gm.GetCommandLog().AddLog($"{room.roomId} repaired", GameManager.ORANGE);
            SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
        }
        room.RepairRoom();
    }

    public void TryToRepairRoom(VillagerData villager,string roomId,int scrapsCost)
    {
        if (GetRoomWithId(roomId).GetVillagersInRoom().Count != 0)
        {
            m_gm.GetCommandLog().AddLogError($"repair {GetRoomWithId(roomId).roomId} failed someone is repairing this room");
            return;
        }
        if (GetRoomWithId(roomId).roomState == RoomData.RoomState.DESTROYED)
        {
            scrapsCost = scrapsCost * 2;
        }
        if (villager.GetWorkingStatus() != VillagerData.WorkingStatus.IDLE)
        {
            m_gm.GetCommandLog().AddLogError($"villager {villager.GetID()} is occupied");
            return;
        }
        if (!m_resourceHandler.HasEnoughResources(0, 0, scrapsCost)) // --Check for ressources
        {
            m_gm.GetCommandLog().AddLogError($"repair {GetRoomWithId(roomId).roomId} failed not enough resources");
            return;
        }
        if (GetRoomWithId(roomId).roomState != RoomData.RoomState.FUNCTIONAL) // --Check if room can be repaired 
        {
            GetRoomWithId(roomId).AddVillagerInRoom(villager); // --Add villager to room
            StartRepairRoom(roomId, scrapsCost); 
        }else{
            m_gm.GetCommandLog().AddLogError($"repair {GetRoomWithId(roomId).roomId} failed");
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
        UpRoomData[] ressRooms = FindObjectsOfType<UpRoomData>();
        foreach (UpRoomData room in ressRooms) { 
            //TODO create a function try to upgrade 
            if (room.roomId == roomId){
                if (m_resourceHandler.HasEnoughResources(0,0, room.upgradeCost)){
                    if (room.roomState == RoomData.RoomState.DESTROYED)
                    {
                        m_gm.GetCommandLog().AddLogError($"upgrade {room.roomId} failed this room is destroyed");
                        return;
                    }
                    UpgradeRoom(room); //Upgrade 
                    m_resourceHandler.ConsumeScraps(room.upgradeCost);
                    m_gm.GetCommandLog().AddLog($"{roomId} upgraded", GameManager.ORANGE);
                    return;
                }
                else{
                    m_gm.GetCommandLog().AddLogError($"upgrade {room.roomId} failed not enough resources");
                    return;
                }
            }else{
                if (roomId == ""){
                    m_gm.GetCommandLog().AddLogError($"specify room id example: upgrade R1");
                    return;
                }        
            }
        }
        m_gm.GetCommandLog().AddLogError($"upgrade {roomId} failed you cant upgrade this room");
        return;
    }

    public void UpgradeRoom(UpRoomData room)
    {
        room.Upgrade();
        if (room is UpVillRoomData){
            UpVillRoomData upRoom = room.GetComponent<UpVillRoomData>();
            UpgradeVillRoom(upRoom);
           
        }
    }

    public void UpgradeVillRoom(UpVillRoomData room)
    {
        switch (room.UpgradeType)
        {
            case UpgradeTypes.REPAIR_SPEED:
                UpgradeRepairSpeed(room);
                break;
            case UpgradeTypes.FATIGUE_REREN:
                UpgradeFatigueRegen(room);
                break;
        }
    }

    public void UpgradeRepairSpeed(UpVillRoomData room)
    {
        IncreaseGlobalRepairSpeed(room.BonusValue);
        m_gm.GetCommandLog().AddLog($"reparation time is faster", GameManager.ORANGE);
    }

    public void UpgradeFatigueRegen(UpVillRoomData room)
    {
        VillagerManager vm = FindObjectOfType<VillagerManager>();
        ReadOnlyCollection<VillagerData> population = vm.GetPopulation();
        foreach (VillagerData villager in population)
        {
            vm.IncreaseRecoveryValue(villager, room.BonusValue);
            Debug.Log(villager.GetName()+"-"+villager.GetRecoveryValue());
        }
        m_gm.GetCommandLog().AddLog($"the population rests faster ", GameManager.ORANGE);
    }

    public void CancelVillRoomUpgrade(UpVillRoomData room)
    {
        switch (room.UpgradeType)
        {
            case UpgradeTypes.REPAIR_SPEED:
                m_repairTimeBonus = 0;
                break;
            case UpgradeTypes.FATIGUE_REREN:
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
        int rng = GameManager.RNG.Next(0,m_roomArray.Length);
        RoomData room = m_roomArray[rng];
        return room;
    }

    public RoomData GetRoomWithId(string roomId) {
        foreach(RoomData room in m_roomArray){
            if(room.roomId == roomId){ return room; }
        }
        return null;
    }

    public RoomData GetRoomOfType(RoomType type)
    {
        RoomData findedRoom = null;
        foreach (RoomData room in m_roomArray){
            if (room.roomType == type){
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

    IEnumerator DegradeRoom()
    {
        RoomData room = PickRandomRoom();
        for (int i = 0;i < m_roomArray.Length; i++)
        {
            room = PickRandomRoom();
            yield return new WaitForSeconds(1);
            if (room.roomState != RoomData.RoomState.DESTROYED)
            {
                room.IncrementDurability(-DEGRADATION);
            }
        }
     
        /*foreach(RoomData room in m_roomArray){  // each room lose durability
            yield return new WaitForSeconds(1);
            room.IncrementDurability(-DEGRADATION);            
        }*/
        yield return new WaitForSeconds(TimeManager.DAY_IN_SECONDS / m_roomArray.Length);
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
    }
    IEnumerator GenerateRessources()
    {
        RessUpRoomData[] ressRooms = FindObjectsOfType<RessUpRoomData>();
        foreach (RessUpRoomData room in ressRooms)
        {
            if (room.upgradeState == RessUpRoomData.UpgradeState.UPGRADED) // check if the room is upgraded
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

    IEnumerator RepairRoomCoroutine(RoomData roomToRepair,float time)
    {
        roomToRepair.SetRoomState(RoomData.RoomState.REPAIRING);
        yield return new WaitForSeconds(time); //TODO Replace this hard value with villager states
        RepairRoomComplete(roomToRepair);
    }
    #endregion

}
