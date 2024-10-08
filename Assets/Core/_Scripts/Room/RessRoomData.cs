using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessRoomData : RoomData
{
    [SerializeField]  int m_ressValue = 1; // generated ressources value
    [SerializeField] float m_generateRessTime = 5; // time to generate ressources
    [SerializeField] RessourceType m_ressourceType;


    public event Action<RessourceType,int> m_ressouceGenerated; 

    public enum RessourceType //TODO REPLACE BY Ressource Type
    {
        RATIONS,
        DRUG,
    }

    //----------- SET GET

    public int ressValue
    {
        get { return m_ressValue; }
    }

    public float genRessTime
    {
        get { return m_generateRessTime; }
    }

    public RessourceType ressourceType
    {
        get { return m_ressourceType; }
    }

    //-----

    void SendRessource(RessourceType ressType, int ressNum)
    {
        m_ressouceGenerated?.Invoke(ressType, ressNum);
    }
}
