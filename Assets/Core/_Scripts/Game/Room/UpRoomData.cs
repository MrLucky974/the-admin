using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpRoomData : RoomData
{

    [SerializeField] UpgradeState m_upgradeState = UpgradeState.NONE;
    int m_upgradeCost = 0;

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

    void SetUpgradeState(UpgradeState newState)
    {
        m_upgradeState = newState;
    }

    public void Upgrade()
    {
        SetUpgradeState(UpgradeState.UPGRADED);
        OnRoomUpgraded?.Invoke();
    }
}
