public class ExpSquad
{
    public enum Status
    {
        PROSPECTING,
        DONE
    }

    private const float EXPLORATION_RATE = 0.1f;

    private int m_identifier;
    private ExpSector m_sector;
    private float m_explorationProgress;

    public ExpSquad(int identifier, ExpSector sector)
    {
        m_identifier = identifier;
        m_explorationProgress = 0f;
        m_sector = sector;
    }

    public Status Update()
    {
        if (m_sector != null && m_explorationProgress < 1f)
        {
            m_explorationProgress += EXPLORATION_RATE;
            if (m_explorationProgress >= 1f)
            {
                var resourceData = m_sector.GetResourceData();
                // TODO : Add resources to handler

                m_sector.Loot();
                m_sector = null;

                return Status.DONE;
            }
        }

        return Status.PROSPECTING;
    }

    public (ExpSector sector, float progress) GetExplorationStatus()
    {
        return (m_sector, m_explorationProgress);
    }

    public int GetIdentifier() { return m_identifier; }
}
