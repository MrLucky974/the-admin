using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RoomData : MonoBehaviour
{
    public const int DEGRADATION = 20; // degradation value 
    public const int MIN_DURABILITY = 0;
    public const int MAX_DURABILITY = 100;


    [SerializeField] protected String m_roomName;
    [SerializeField] protected String m_roomId;
    protected int m_durability = 100; // room hp 
    protected int m_maxDurability = MAX_DURABILITY; // max room hp

    protected int m_repairCost = 1;
    protected ArrayList m_villagerInRoom = new ArrayList(); 

    public event Action<int> OnRoomRepaired;
    public event Action<int> OnDurabilityChanged;
    public event Action OnStateChanged;
    public event Action<RoomData> OnRoomDestroyed;


    [SerializeField] protected RoomType m_roomType;
    
    
    protected RoomState m_roomState = RoomState.FUNCTIONAL;
    public enum RoomState {
        FUNCTIONAL,
        DAMAGED,
        REPAIRING,
        DESTROYED
    };

    RoomManager m_roomManager;

    //----------- SET GET

    public String roomId
    {
        set { m_roomId = value; }
        get { return m_roomId; }
    }

    public RoomState roomState
    {
        get { return m_roomState;}
    }

    public int durability
    {
        get { return m_durability; }
    }

    public int villagerCount
    {
        get { return m_villagerInRoom.Count; }
    }

    public RoomType roomType
    {
        get { return m_roomType; }
    }

    public ArrayList GetVillagersInRoom() => m_villagerInRoom;
  

    //--

    private void Awake()
    {
        //m_durability = m_maxDurability;
        gameObject.name = m_roomName;
        m_roomManager = FindObjectOfType<RoomManager>();
        OnDurabilityChanged?.Invoke(m_durability);
    }

    public void SetRoomState(RoomState newState)
    {
        m_roomState = newState;
        OnStateChanged?.Invoke();
    }
    public void RepairRoom() // repair the room
    {
        OnRoomRepaired?.Invoke(m_repairCost);
        m_durability = m_maxDurability;
        OnDurabilityChanged?.Invoke(m_durability);
        SetRoomState(RoomState.FUNCTIONAL);
        RemoveAllVillager();
    }

    public void IncrementDurability(int value){ 
        m_durability += value;
        OnDurabilityChanged?.Invoke(m_durability);
        m_durability = Mathf.Clamp(m_durability,0,m_maxDurability);
        CheckIsDamaged();
        CheckIsDestroy();
    }

    protected void CheckIsDamaged(){ 
        if(m_durability <= m_maxDurability / 2)
        {
            SetRoomState(RoomState.DAMAGED);
        }
    }

    protected void CheckIsDestroy(){
        if (roomState == RoomState.DESTROYED) return;
        if (m_durability <= 0){
            DestroyRoom();           
        }
    }


    #region Handling Villager In Room
    public void AddVillagerInRoom(VillagerData villager)
    {
        m_villagerInRoom.Add(villager);
    }
    #endregion


    public void RemoveAllVillager()
    {
        m_villagerInRoom.Clear();
    }

    virtual protected void DestroyRoom(){
        SetRoomState(RoomState.DESTROYED);
        OnRoomDestroyed?.Invoke(this);
    }
  
}
