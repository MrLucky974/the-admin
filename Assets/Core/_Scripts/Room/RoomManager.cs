
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Specialized.BitVector32;
using static UnityEngine.Rendering.HDROutputUtils;


public class RoomManager : MonoBehaviour
{

    Test m_admin;

    RoomData[] m_roomArray;
    ArrayList m_ids = new ArrayList();

    ArrayList m_roomNames = new ArrayList();


    Coroutine m_degradeCoroutine;
    Coroutine m_ressCoroutine;

    const int DEGRADATION = 5;


    void Start()
    {
        m_roomArray = FindObjectsOfType<RoomData>();//GetComponentsInChildren<RoomData>(); 
        InitIds(m_roomArray.Length-1);
        m_admin = FindObjectOfType<Test>();
        m_degradeCoroutine = StartCoroutine(DegradeRoom());
        m_ressCoroutine = StartCoroutine(GenerateRessources());
    }

    void Update()
    {
        
    }


    void InitIds(int roomNum)
    {
        Debug.Log(m_ids.Count);
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
        Debug.Log("fini "+m_ids.Count);
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
        RessRoomData[] ressRooms = FindObjectsOfType<RessRoomData>();
        foreach (RessRoomData room in ressRooms)
        {
            m_admin.AddRessourceSignal(room.ressourceType,room.ressValue);
            yield return new WaitForSeconds(room.genRessTime);
        }
        yield return new WaitForSeconds(1);
        m_ressCoroutine = StartCoroutine(GenerateRessources());

    }

}
