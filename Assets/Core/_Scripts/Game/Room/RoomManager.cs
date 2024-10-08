
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.HDROutputUtils;


public class RoomManager : MonoBehaviour
{

    Test m_admin;


    ResourceHandler m_resourceHandler;
    RoomData[] m_roomArray;
    ArrayList m_ids = new ArrayList();

    ArrayList m_roomNames = new ArrayList();


    Coroutine m_degradeCoroutine;
    Coroutine m_ressCoroutine;

    const int DEGRADATION = 5;


    public void Initialize()
    {
        m_roomArray = FindObjectsOfType<RoomData>();//GetComponentsInChildren<RoomData>(); 
        m_resourceHandler = FindObjectOfType<ResourceHandler>();
        InitIds(m_roomArray.Length-1);
        m_admin = FindObjectOfType<Test>();
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
        m_ressCoroutine = StartCoroutine(GenerateRessources());
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump")){
            UpgradeRoom("R2");
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
        foreach (RoomData room in m_roomArray)
        {
            int index = Random.Range(0, m_ids.Count);
            room.roomId = "R" + m_ids[index];
            m_ids.RemoveAt(index);
            Debug.Log(room.name+" / "+room.roomId);
        }
    }

    public void UpgradeRoom(string roomId)
    {
        UpRoomData[] ressRooms = FindObjectsOfType<RessUpRoomData>();
        foreach (UpRoomData room in ressRooms) {
            if (room.roomId == roomId)
            {
                GameManager gm = FindObjectOfType<GameManager>();
                if (m_resourceHandler.HasEnoughResources(0,0, room.upgradeCost))
                {
                    room.Upgrade();
                    m_resourceHandler.ConsumeScraps(room.upgradeCost);
                    gm.GetCommandLog().AddLog($"{roomId} upgraded", GameManager.ORANGE);
                }
                else{  
                    gm.GetCommandLog().AddLog($"upgrade {room.roomId} failed not enough resources", GameManager.RED);
                }
            } 
        }
    }


    IEnumerator DegradeRoom()
    {
        foreach(RoomData room in m_roomArray){
            if (room.roomState != RoomData.RoomState.DESTROYED){
                yield return new WaitForSeconds(1);
                room.IncrementDurability(-DEGRADATION);  
            }
        }
        yield return new WaitForSeconds(1);
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
    }



    IEnumerator GenerateRessources()
    {
        RessUpRoomData[] ressRooms = FindObjectsOfType<RessUpRoomData>();
        foreach (RessUpRoomData room in ressRooms)
        {
            if (room.upgradeState == RessUpRoomData.UpgradeState.UPGRADED) // check if the room is upgraded
            {
                switch (room.ressourceType)
                {
                    case ResourceType.RATIONS:
                        m_resourceHandler.AddRations(room.ressValue);
                        break;
                    case ResourceType.MEDS:
                        m_resourceHandler.AddMeds(room.ressValue);
                        break;
                }
            }
            yield return new WaitForSeconds(room.genRessTime);
        }
        yield return new WaitForSeconds(1);
        m_ressCoroutine = StartCoroutine(GenerateRessources());
    }

}
