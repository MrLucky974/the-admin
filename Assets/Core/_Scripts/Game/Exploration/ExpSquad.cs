public class ExpSquad
{
    public static ExpSquad Generate(ExpSector sector)
    {
        var squad = new ExpSquad(sector);

        return squad;
    }

    private readonly ExpSector m_sector;
    private float m_explorationProgress;

    private ExpSquad(ExpSector sector)
    {
        m_explorationProgress = 0f;
        m_sector = sector;
    }

    public (ExpSector sector, float progress) GetExplorationStatus()
    {
        return (m_sector, m_explorationProgress);
    }
}
