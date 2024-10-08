using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RoomData : MonoBehaviour
{
    [SerializeField] protected String m_roomName;
    [SerializeField] protected String m_roomId;
    protected int m_durability = 100; // room hp 
    protected int m_maxDurability = 100; // max room hp


    protected const int DEGRADATION = 20; // degradation value 
    protected int m_repairCost = 5;
    protected Array m_villagerInRoom; // !! replace with peapleData type

    protected RoomState m_roomState = RoomState.FUNCTIONAL;

    public event Action<int> OnRoomRepaired;
    public event Action<int> OnDurabilityChanged;



    public enum RoomState {
        FUNCTIONAL,
        DAMAGED,
        DESTROYED
    };

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
    //--

    private void Awake()
    {
        m_durability = m_maxDurability;
        gameObject.name = m_roomName;
        OnDurabilityChanged?.Invoke(m_durability);
    }
    private void Update()
    {
        //Debug.Log(m_roomState+" / "+m_durability);
        //TODO REMOVE update
        if (Input.GetButtonDown("Fire1")){
            IncrementDurability(-5);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            RepairRoom();
        }
    }
    protected void SetRoomState(RoomState newState)
    {
        m_roomState = newState;
    }
    public void RepairRoom() // repair the room
    {
        OnRoomRepaired?.Invoke(m_repairCost);
        m_durability = m_maxDurability;
        OnDurabilityChanged?.Invoke(m_durability);
        SetRoomState(RoomState.FUNCTIONAL);
    }

    public void IncrementDurability(int value){ 
        m_durability += value;
        OnDurabilityChanged?.Invoke(m_durability);
        m_durability = Mathf.Clamp(m_durability,0,m_maxDurability);
        CheckIsDamaged();
        CheckIsDestroy();
    }
    protected void CheckIsDamaged(){ 
        if(m_durability < m_maxDurability / 2)
        {
            SetRoomState(RoomState.DAMAGED);
        }
    }
    protected void CheckIsDestroy(){
        if (m_durability <= 0){
            DestroyRoom();           
        }
    }

    protected void DestroyRoom(){
        SetRoomState(RoomState.DESTROYED);
        Debug.Log("BOOM");
    }
  
}
