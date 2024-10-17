using System.Text;
using UnityEngine;

public class ExplorationGridDisplay : MonoBehaviour
{
    [SerializeField] private TextGrid m_grid;

    private ExplorationSystem m_explorer;

    private void Start()
    {
        m_explorer = GameManager.Instance.GetExplorer();
        m_explorer.OnRegionRegeneration += HandleRegionRegeneration;
        m_explorer.OnSectorScanned += HandleScannedSector;
        m_explorer.OnSquadStatusChanged += HandleSquadStatus;
    }

    private void OnDestroy()
    {
        m_explorer.OnSectorScanned -= HandleScannedSector;
    }

    private void HandleRegionRegeneration()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                m_grid.SetCell(x, y, "");
            }
        }
    }

    private void HandleSquadStatus(ExplorationSystem.SquadStatusEventData data)
    {
        StringBuilder sb = new StringBuilder();
        if (data.isSectorScanned)
        {
            sb.AppendLine(JUtils.FormatColor($"{data.resource}", GameManager.GREEN));
        }

        Debug.Log(data.finishedExploration);
        if (data.finishedExploration is false)
        {
            sb.AppendLine(JUtils.FormatColor(JUtils.GenerateTextSlider(data.progress), GameManager.ORANGE));
        }

        m_grid.SetCell(data.x, data.y, sb.ToString());
    }

    private void HandleScannedSector(ExplorationSystem.SectorEventData data)
    {
        var text = JUtils.FormatColor($"{data.resource}", GameManager.GREEN);
        m_grid.SetCell(data.x, data.y, text);
    }
}
