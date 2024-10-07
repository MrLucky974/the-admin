using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabOption : MonoBehaviour
{
    [SerializeField] private GameObject m_defaultGo;
    [SerializeField] private GameObject m_selectedGo;

    [Space]

    [SerializeField] private TabPanel m_panel;

    public void Toggle(bool selected)
    {
        m_selectedGo.SetActive(selected);
        m_defaultGo.SetActive(!selected);
        if (m_panel != null)
        {
            m_panel.Toggle(selected);
        }
    }
}
