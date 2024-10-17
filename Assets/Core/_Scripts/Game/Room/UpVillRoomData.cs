using UnityEngine;

public class UpVillRoomData : UpRoomData
{

    int m_bonusValue = 1;
    [SerializeField] UpgradeTypes m_upgradeTypes;


    public enum UpgradeTypes
    {
        REPAIR_SPEED,
        FATIGUE_REREN
    }


    public int BonusValue
    {
        get { return m_bonusValue; }
    }

    public UpgradeTypes UpgradeType
    {
        get { return m_upgradeTypes; }
    }
}

