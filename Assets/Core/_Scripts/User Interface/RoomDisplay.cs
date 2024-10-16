using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomDisplay : MonoBehaviour
{
    [SerializeField] protected RoomType m_roomType;
    [SerializeField] protected TextMeshProUGUI m_tmPro;
    [SerializeField] protected RoomManager m_rm;
    protected RoomData m_room;

    protected bool m_isInit = false;

    private void Awake()
    {
        m_rm = FindObjectOfType<RoomManager>();
        m_tmPro = gameObject.GetComponent<TextMeshProUGUI>();
        Init();
    }



    protected void Init()
    {
        CheckChild();
        m_room = m_rm.GetRoomOfType(m_roomType);
        m_room.OnStateChanged += ChangeColor;  
        m_tmPro.color = GameManager.GREEN;
        m_tmPro.alignment = TextAlignmentOptions.Center;
        DisplayName();
        m_isInit=true;
    }

    protected void CheckChild()
    {
        RoomDisplay[] childs = GetComponentsInChildren<RoomDisplay>();
        if (childs.Length > 0)
        {
            foreach (RoomDisplay child in childs)
            {
                child.SetRoomType(m_roomType);
            }
        }
    }

    protected void SetRoomType(RoomType newType)
    {
        m_roomType = newType;
    }


    protected void OnEnable()
    {
        if (!m_isInit) { Init(); }
        ChangeColor();
    }

    protected void ChangeColor()
    {
        switch (m_room.roomState) {
            case RoomData.RoomState.FUNCTIONAL:
                m_tmPro.color = GameManager.GREEN;
                break;
            case RoomData.RoomState.DAMAGED:
                m_tmPro.color = GameManager.ORANGE;
                break;
            case RoomData.RoomState.DESTROYED:
                m_tmPro.color = GameManager.RED;
                break;
        }
    }

    void DisplayName()
    {
        m_tmPro.SetText(m_room.name); 
    }
}
