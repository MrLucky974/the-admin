/// <summary>
/// Represents an enemy with a strength value and location.
/// </summary>
public class Enemy
{
    private int m_strength;
    private string m_location;
    private bool m_inCombat;

    public Enemy(int strength, string location)
    {
        m_strength = strength;
        m_location = location;
    }

    public int GetStrength()
    {
        return m_strength;
    }

    public string GetLocation()
    {
        return m_location;
    }

    public void SetLocation(string newSectorIdentifier)
    {
        m_location = newSectorIdentifier;
    }

    public bool InCombat() => m_inCombat;

    public void MarkAsInCombat()
    {
        m_inCombat = true;
    }

    public void StopCombat()
    {
        m_inCombat = false;
    }

    public override string ToString()
    {
        return $"Enemy at {m_location} with strength {m_strength}";
    }
}