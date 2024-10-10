using System;
using System.Collections.Generic;
using UnityEngine;

// TODO : Move this into its own file
public enum StatusPageIndex
{
    STOCK = 0,
    CHECKUP = 1,
}

public class PageSwitcher : MonoBehaviour
{
    [SerializeField] private bool m_hideAllOnStart = true;
    [SerializeField] private List<GameObject> m_pages;

    private void Start()
    {
        if (m_hideAllOnStart)
            HideAll();
    }

    public void Select<TEnum>(TEnum page) where TEnum : Enum
    {
        // Convoluted mess to be able to use any enum as a substitute for an int
        Array values = Enum.GetValues(typeof(TEnum));
        int index = -1;
        for (int i = 0; i < values.Length; i++)
        {
            var value = (TEnum)values.GetValue(i);
            if (page.Equals(value))
            {
                index = i;
            }
        }

        if (index == -1)
        {
            Debug.LogError("No value found", this);
            return;
        }
        Select(index);
    }

    public void Select(int index)
    {
        if (m_pages == null)
            return;
        
        if (index < 0 || index >= m_pages.Count)
        {
            throw new System.ArgumentOutOfRangeException(nameof(index));
        }

        HideAll();
        for (int i = 0; i < m_pages.Count; i++)
        {
            if (index == i)
            {
                m_pages[i].SetActive(true);
                return;
            }
        }
    }

    public void HideAll()
    {
        foreach (var page in m_pages)
        {
            page.gameObject.SetActive(false);
        }
    }
}
