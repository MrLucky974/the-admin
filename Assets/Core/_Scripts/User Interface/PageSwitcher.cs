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
    private const int INVALID_PAGE_INDEX = -1;

    [SerializeField] private bool m_hideAllOnStart = true;
    [SerializeField] private List<GameObject> m_pages;

    private int m_selectedIndex = INVALID_PAGE_INDEX;

    private void Start()
    {
        if (m_hideAllOnStart)
            HideAll();
    }

    public bool IsPageSelected()
    {
        return m_selectedIndex != INVALID_PAGE_INDEX;
    }

    public int GetSelectedIndex()
    {
        return m_selectedIndex;
    }

    public TEnum GetSelectedIndex<TEnum>() where TEnum : Enum
    {
        int index = GetSelectedIndex();
        Array values = Enum.GetValues(typeof(TEnum));
        var value = (TEnum)values.GetValue(index);
        return value;
    }

    public void Select<TEnum>(TEnum page) where TEnum : Enum
    {
        // Convoluted mess to be able to use any enum as a substitute for an int
        Array values = Enum.GetValues(typeof(TEnum));
        int index = INVALID_PAGE_INDEX;
        for (int i = 0; i < values.Length; i++)
        {
            var value = (TEnum)values.GetValue(i);
            if (page.Equals(value))
            {
                index = i;
            }
        }

        if (index == INVALID_PAGE_INDEX)
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
                break;
            }
        }

        m_selectedIndex = index;
    }

    public void HideAll()
    {
        foreach (var page in m_pages)
        {
            page.gameObject.SetActive(false);
        }

        m_selectedIndex = INVALID_PAGE_INDEX;
    }
}
