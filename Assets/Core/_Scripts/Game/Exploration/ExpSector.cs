using System;
using System.Text;

public class ExpSector
{
    private readonly string m_identifier;
    private ResourceType m_resourceType;
    private int m_amount = 0;

    public ExpSector(string identifier)
    {
        m_identifier = identifier;
    }

    public (ResourceType resourceType, int amount) GetResourceData()
    {
        return (m_resourceType, m_amount);
    }

    public static ExpSector Generate(string identifier)
    {
        var sector = new ExpSector(identifier);

        var rng = GameManager.RNG;

        // Select resource type
        Array values = Enum.GetValues(typeof(ResourceType));
        int randomResourceType = rng.Next(values.Length);
        sector.m_resourceType = (ResourceType)values.GetValue(randomResourceType);

        // Generate resource amount
        if (sector.m_resourceType != ResourceType.NONE)
        {
            sector.m_amount = DistributeResources(rng);
        }

        return sector;
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
