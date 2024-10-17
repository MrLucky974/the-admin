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

    public event Action<ResourceType, int> m_addRessource;

    void Start()
    {

    }


    public void AddRessourceSignal(ResourceType ressType, int ressNum)
    {
        m_addRessource?.Invoke(ressType, ressNum);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
