using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationTest : MonoBehaviour
{
    private ExpRegion m_region;

    private void Start()
    {
        m_region = ExpRegion.Generate();
        int size = m_region.GetSize();
        for (int i = 0; i < size; i++)
        {
            string identifier = m_region.GetIdentifier(i);
            var sector = m_region.GetSector(identifier);
            Debug.Log(sector);
        }
    }
}
