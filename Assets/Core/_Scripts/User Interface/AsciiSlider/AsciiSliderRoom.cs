using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsciiSliderRoom : AsciiSlider
{
    //Use to display room durability
    [SerializeField] RoomData m_ownerRoom;


    new void Start()
    {
        base.Start();
        m_ownerRoom.OnDurabilityChanged += SetSlider;
    }

    new void SetSlider(int value)
    {
        m_value = value / 10;
        base.SetSlider(m_value);
    }

}
