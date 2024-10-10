using System.Text;

/// <summary>
/// Data class containing data for exploration gameplay elements (resources gathering).
/// </summary>
public class Sector
{
    public static Sector Generate(string identifier)
    {
        var sector = new Sector(identifier);

        var rng = GameManager.RNG;

        // Select resource type
        (ResourceType type, int chance)[] values = {
            (ResourceType.NONE, 90),
            (ResourceType.RATIONS, 20),
            (ResourceType.MEDS, 10),
            (ResourceType.SCRAPS, 20),
        };

        int weightedSum = 0;
        foreach (var (_, chance) in values)
        {
            weightedSum += chance;
        }

        int r = rng.Next(weightedSum);
        foreach (var (type, chance) in values)
        {
            if (r < chance && r > 0)
            {
                sector.m_resourceType = type;
                break;
            }

            r -= chance;
        }

        // Generate resource amount
        if (sector.m_resourceType != ResourceType.NONE)
        {
            sector.m_amount = DistributeResources(rng);
        }

        return sector;
    }

    private readonly string m_identifier;
    private ResourceType m_resourceType;
    private int m_amount = 0;
    private bool m_isLooted;

    private Sector(string identifier)
    {
        m_identifier = identifier;
    }

    public (ResourceType resourceType, int amount) GetResourceData()
    {
        return (m_resourceType, m_amount);
    }

    public bool IsLooted() { return m_isLooted; }

    public void Loot()
    {
        if (m_isLooted)
            return;

        m_resourceType = ResourceType.NONE;
        m_amount = 0;
        m_isLooted = true;
    }

    private static int DistributeResources(System.Random rng)
    {
        const int rollCount = 6;
        var rolls = new int[rollCount];

        int count = 0;
        for (int i = 0; i < rollCount; i++)
        {
            count += rolls[i] = JRandom.RollDice(2, 3, rng);
            count += rolls[i] = JRandom.RollDice(2, 3, rng);
        }
        count = 1 + JMath.Min(rolls);

        //rolls.Print();

        return count;
    }

    public string GetIdentifier()
    {
        return m_identifier;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(m_identifier);
        sb.Append('(');
        sb.Append(m_resourceType);
        if (m_resourceType != ResourceType.NONE)
        {
            sb.Append($": {m_amount}");
        }
        sb.Append(')');
        return sb.ToString();
    }
}
