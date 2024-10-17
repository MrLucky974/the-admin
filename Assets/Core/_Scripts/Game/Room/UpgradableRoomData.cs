using System;
using UnityEngine;

public class UpgradableRoomData : RoomData
{
    public const int COST_FACTOR = 5;

    [SerializeField] UpgradeState m_upgradeState = UpgradeState.NONE;
    [SerializeField] int m_upgradeCost = 5;
    string m_message;

    public enum UpgradeState
    {
        NONE,
        UPGRADED
    }

    public event Action OnRoomUpgraded;

    public int upgradeCost
    {
        get { return m_upgradeCost; }
    }

    public UpgradeState upgradeState
    {
        get { return m_upgradeState; }
    }

    public void SetUpgradeState(UpgradeState newState)
    {
        m_upgradeState = newState;
    }

    void IncreaseUpradeCost(int amount)
    {
        m_upgradeCost += amount;

    }

    void DecreaseUpradeCost(int amount)
    {
        m_upgradeCost -= amount;
    }

    virtual public void Upgrade()
    {
        SetUpgradeState(UpgradeState.UPGRADED);
        IncreaseUpradeCost(COST_FACTOR);
        OnRoomUpgraded?.Invoke();
    }

    override protected void DestroyRoom()
    {
        base.DestroyRoom();
        m_upgradeCost = COST_FACTOR;
    }
}
