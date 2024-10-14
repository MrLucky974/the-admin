using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomIdDisplay : RoomDisplay
{
    string m_id;
    private void Awake()
    {
        m_rm = FindObjectOfType<RoomManager>();
        m_tmPro = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        Init();
    }

    new void Init()
    {
        base.Init();
        DisplayId();
       
    }

    void DisplayId()
    {
        string newId = m_room.roomId;
        m_tmPro.SetText($"[{newId}]");
    }



}
