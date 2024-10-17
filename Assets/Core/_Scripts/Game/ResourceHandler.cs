using System;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    // Minimum and maximum values for resources
    public const int MIN_RESOURCE = 0;
    public const int MAX_RESOURCE = 999;

    private int m_rations; // Vivres
    private int m_meds; // Soins
    private int m_scraps; // Pièces détachés

    // Events that other components can subscribe to
    public event Action<int> OnRationsChanged;
    public event Action<int> OnMedsChanged;
    public event Action<int> OnScrapsChanged;

    public void Initialize()
    {
        OnRationsChanged?.Invoke(m_rations);
        OnMedsChanged?.Invoke(m_meds);
        OnScrapsChanged?.Invoke(m_scraps);

        // Initialize resources
        AddRations(24);
        AddMeds(14);
        AddScraps(20);
    }

    // Properties with get/set methods
    public int Rations
    {
        get => m_rations;
        private set
        {
            m_rations = Mathf.Clamp(value, MIN_RESOURCE, MAX_RESOURCE);
            OnRationsChanged?.Invoke(m_rations);
        }
    }

    public int Meds
    {
        get => m_meds;
        private set
        {
            m_meds = Mathf.Clamp(value, MIN_RESOURCE, MAX_RESOURCE);
            OnMedsChanged?.Invoke(m_meds);
        }
    }

    public int Scraps
    {
        get => m_scraps;
        private set
        {
            m_scraps = Mathf.Clamp(value, MIN_RESOURCE, MAX_RESOURCE);
            OnScrapsChanged?.Invoke(m_scraps);
        }
    }

    // Methods to modify resources
    public bool AddRations(int amount)
    {
        if (amount < 0) return false;
        Rations += amount;
        return true;
    }

    public bool AddMeds(int amount)
    {
        if (amount < 0) return false;
        Meds += amount;
        return true;
    }

    public bool AddScraps(int amount)
    {
        if (amount < 0) return false;
        Scraps += amount;
        return true;
    }

    public bool ConsumeRations(int amount)
    {
        if (amount < 0 || m_rations < amount) return false;
        Rations -= amount;
        return true;
    }

    public bool ConsumeMeds(int amount)
    {
        if (amount < 0 || m_meds < amount) return false;
        Meds -= amount;
        return true;
    }

    public bool ConsumeScraps(int amount)
    {
        if (amount < 0 || m_scraps < amount) return false;
        Scraps -= amount;
        return true;
    }

    // Method to check if there are enough resources
    public bool HasEnoughResources(int rationsNeeded, int medsNeeded, int scrapsNeeded)
    {
        return m_rations >= rationsNeeded &&
               m_meds >= medsNeeded &&
               m_scraps >= scrapsNeeded;
    }

    // Method to consume multiple resources at once
    public bool ConsumeResources(int rationsAmount, int medsAmount, int scrapsAmount)
    {
        if (!HasEnoughResources(rationsAmount, medsAmount, scrapsAmount))
            return false;

        ConsumeRations(rationsAmount);
        ConsumeMeds(medsAmount);
        ConsumeScraps(scrapsAmount);
        return true;
    }
}
