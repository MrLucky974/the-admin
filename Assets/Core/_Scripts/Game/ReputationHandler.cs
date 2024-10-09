using System;
using UnityEngine;

public class ReputationHandler : MonoBehaviour
{
    public const int MIN_REPUTATION = -100;
    public const int MAX_REPUTATION = 100;

    private int m_reputation;

    public event Action<int> OnReputationChanged;

    public void Initialize()
    {
        OnReputationChanged?.Invoke(m_reputation);
    }

    public int Reputation
    {
        get => m_reputation;
        private set
        {
            m_reputation = Mathf.Clamp(value, MIN_REPUTATION, MAX_REPUTATION);
            OnReputationChanged?.Invoke(m_reputation);
        }
    }

#if UNITY_EDITOR

    public void SetReputation(int value)
    {
        Reputation = value;
    }

#endif

    public bool IncreaseReputation(int amount)
    {
        if (amount < 0) return false;
        Reputation += amount;
        return true;
    }

    public bool DecreaseReputation(int amount)
    {
        if (amount < 0) return false;
        Reputation -= amount;
        return true;
    }
}
