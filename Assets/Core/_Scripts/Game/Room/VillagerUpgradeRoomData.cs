using UnityEngine;

public class VillagerUpgradeRoomData : UpgradableRoomData
{
    public enum UpgradeTypes
    {
        REPAIR_SPEED,
        FATIGUE_REGEN
    }

    int m_bonusValue = 1;
    [SerializeField] UpgradeTypes m_upgradeType;

    public int BonusValue => m_bonusValue;

    public UpgradeTypes UpgradeType => m_upgradeType;
}

