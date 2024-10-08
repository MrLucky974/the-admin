using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessUpRoomData : UpRoomData
{
    [SerializeField]  int m_ressValue = 1; // generated ressources value
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

    void SendRessource(ResourceType ressType, int ressNum)
    {
        OnResourceGenerated?.Invoke(ressType, ressNum);
    }
}
