using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;

public class UpVillRoomData : UpRoomData
{

    int BONUS_VALUE = 1;
    [SerializeField ] UpgradeTypes m_upgradeTypes;


    public enum UpgradeTypes
    {
        REPAIR_SPEED,
        FATIGUE_REREN
    }
       
        

    override public void Upgrade()
    {
        base.Upgrade();
        switch(m_upgradeTypes)
        {
            case UpgradeTypes.REPAIR_SPEED:
                IncreaseRepairSpeed();
                break;
            case UpgradeTypes.FATIGUE_REREN:

                break;
        }
       
    }  
    
    public void IncreaseRepairSpeed()
    {
        RoomManager rm = FindObjectOfType<RoomManager>();
        rm.IncreaseGlobalRepairSpeed(BONUS_VALUE);
    }

    public void IncreaseFatigueRegen()
    {

    }
}
