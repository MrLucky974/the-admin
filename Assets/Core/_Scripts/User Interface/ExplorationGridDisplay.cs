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
        m_explorer.OnCurrentSectorScan += HandleCurrentScan;
        m_explorer.OnSquadStatusChanged += HandleSquadStatus;
    }

    private void OnDestroy()
    {
        m_explorer.OnRegionRegeneration -= HandleRegionRegeneration;
        m_explorer.OnSectorScanned -= HandleScannedSector;
        m_explorer.OnCurrentSectorScan -= HandleCurrentScan;
        m_explorer.OnSquadStatusChanged -= HandleSquadStatus;
    }

    private void HandleCurrentScan(ExplorationSystem.SectorEventData data)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(JUtils.FormatColor("SCAN", GameManager.ORANGE));
        sb.AppendLine(JUtils.FormatColor(JUtils.GenerateTextSlider(data.Progress, 6), GameManager.ORANGE));
        m_grid.SetCell(data.X, data.Y, sb.ToString());
    }

    private void HandleRegionRegeneration()
    {
        m_grid.ClearCells();
    }

    private void HandleSquadStatus(ExplorationSystem.SquadStatusEventData data)
    {
        StringBuilder sb = new StringBuilder();
        if (data.isSectorScanned)
        {
            sb.AppendLine(JUtils.FormatColor($"{data.resource}", GameManager.GREEN));
        }

        if (data.finishedExploration is false)
        {
            sb.AppendLine(JUtils.FormatColor(JUtils.GenerateTextSlider(data.progress, 6), GameManager.ORANGE));
        }

        m_grid.SetCell(data.x, data.y, sb.ToString());
    }

    private void HandleScannedSector(ExplorationSystem.SectorEventData data)
    {
        var text = JUtils.FormatColor($"{data.Resource}", GameManager.GREEN);
        m_grid.SetCell(data.X, data.Y, text);
    }
}
