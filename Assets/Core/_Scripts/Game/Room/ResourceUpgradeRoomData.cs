using System;
using UnityEngine;

public class ResourceUpgradeRoomData : UpgradableRoomData
{
    [SerializeField] int m_ressValue = 1; // generated ressources value
    [SerializeField] float m_generateRessTime = 5; // time to generate ressources
    [SerializeField] ResourceType m_ressourceType;

    public event Action<ResourceType, int> OnResourceGenerated;

    //----------- SET GET
    public int ressValue
    {
        get { return m_ressValue; }
    }

    public float genRessTime
    {
        get { return m_generateRessTime; }
    }

    public ResourceType ressourceType
    {
        get { return m_ressourceType; }
    }
    //-----

    void SendResource(ResourceType ressType, int ressNum)
    {
        OnResourceGenerated?.Invoke(ressType, ressNum);
    }

    protected override void DestroyRoom()
    {
        base.DestroyRoom();
        SetUpgradeState(UpgradeState.NONE);
    }
}
