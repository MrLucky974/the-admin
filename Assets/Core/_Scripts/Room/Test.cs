using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] int m_drug;
    [SerializeField] int m_ration;

    public event Action<RessRoomData.RessourceType, int> m_addRessource;

    void Start()
    {
        m_addRessource += AddRessource;
    }


    public void AddRessourceSignal(RessRoomData.RessourceType ressType, int ressNum)
    {
        m_addRessource?.Invoke(ressType, ressNum);
    }

    void AddRessource(RessRoomData.RessourceType ressType, int ressNum)
    {
        switch (ressType)
        {
            case RessRoomData.RessourceType.RATIONS:
                m_ration += ressNum; break;
            case RessRoomData.RessourceType.DRUG:
                m_drug += ressNum; break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
