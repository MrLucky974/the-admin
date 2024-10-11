
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.HDROutputUtils;
using System;


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

    public const int DEGRADATION = 7;
    public const float TIMEDEGRADE = 1f;

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
    }

    void InitRoom()
    {
        InitIds(m_roomArray.Length - 1);
        foreach (RoomData room in m_roomArray)
        {
            room.OnRoomDestroyed +=RoomWasDestroy;
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

    #region Room Handling Utilities

    public void RoomWasDestroy()
    {
        m_reputationHandler.DecreaseReputation(15); //TODO add a reputation value
        Debug.Log("AIIIIIIE");
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
                float repairSpeed = VillagerData.DEFAULT_WORKING_SPEED;
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
            vm.IncreaseFatigue(villager,5); //Increase fatigue to villager in room //TODO compute de fatigue value
            vm.SetWorkingStatus(villager, VillagerData.WorkingStatus.IDLE);
        }
        room.RepairRoom();
    }

    public void TryToRepairRoom(VillagerData villager,string roomId,int scrapsCost)
    {
        if (villager.GetWorkingStatus() != VillagerData.WorkingStatus.IDLE)
        {
            m_gm.GetCommandLog().AddLog($"villager {villager.GetID()} is occupied", GameManager.RED);
            return;
        }
        if (!m_resourceHandler.HasEnoughResources(0, 0, scrapsCost)) // --Check for ressources
        {
            m_gm.GetCommandLog().AddLog($"repair {GetRoomWithId(roomId).roomId} failed not enough resources", GameManager.RED);
            return;
        }
        if (GetRoomWithId(roomId).roomState == RoomData.RoomState.DAMAGED) // --Check if room can be repaired 
        {
            GetRoomWithId(roomId).AddVillagerInRoom(villager); // --Add villager to room
            StartRepairRoom(roomId, scrapsCost); 
        }else{
            m_gm.GetCommandLog().AddLog($"repair {GetRoomWithId(roomId).roomId} failed", GameManager.RED);
            return;
        }
    }

    public void AddVillagerToRoom(VillagerData villager, string roomId)
    {
        GetRoomWithId(roomId).AddVillagerInRoom(villager);
    }

    public void UpgradeRoom(string roomId)
    {
        UpRoomData[] ressRooms = FindObjectsOfType<RessUpRoomData>();
        foreach (UpRoomData room in ressRooms) {
            if (room.roomId == roomId){
                if (m_resourceHandler.HasEnoughResources(0,0, room.upgradeCost)){
                    room.Upgrade();
                    m_resourceHandler.ConsumeScraps(room.upgradeCost);
                    m_gm.GetCommandLog().AddLog($"{roomId} upgraded", GameManager.ORANGE);
                    return;
                }
                else{
                    m_gm.GetCommandLog().AddLog($"upgrade {room.roomId} failed not enough resources", GameManager.RED);
                    return;
                }
            }else{
                if (roomId == ""){
                    m_gm.GetCommandLog().AddLog($"specify room id example: upgrade R1", GameManager.RED);
                    return;
                }
                m_gm.GetCommandLog().AddLog($"upgrade {roomId} failed you cant upgrade this room", GameManager.RED);
                return;
            }
        }
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
        foreach(RoomData room in m_roomArray){
            if (room.roomState != RoomData.RoomState.DESTROYED){
                yield return new WaitForSeconds(1);
                room.IncrementDurability(-DEGRADATION);  
            }
        }
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
        //roomToRepair.RepairRoom();
        m_gm.GetCommandLog().AddLog($"{roomToRepair.roomId} repaired", GameManager.ORANGE);
    }
    #endregion

}
