using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationSystem : MonoBehaviour
{
    private CommandLogManager m_commandLog;

    private ExpRegion m_region;

    // Scanning
    private ExpSector m_currentSectorScan;
    private Coroutine m_scanCoroutine;

    private List<ExpSector> m_scannedSectors;

    // Exploration


    public void Initialize()
    {
        m_scannedSectors = new List<ExpSector>();

        var commandSystem = GameManager.Instance.GetCommands();
        m_commandLog = GameManager.Instance.GetCommandLog();
        
        commandSystem.AddCommand(new CommandDefinition<Action>("region", () =>
        {
            // TODO : Open modal box when region is not explored at 100%
            m_region = ExpRegion.Generate();
        }));

        commandSystem.AddCommand(new CommandDefinition<Action<string>>("scan", (string identifier) =>
        {
            // TODO : Open modal box when sector is not yet scanned
            ExpSector selectedSector = m_region.GetSector(identifier);
            if (selectedSector != null)
            {
                foreach (var sector in m_scannedSectors)
                {
                    if (sector.GetIdentifier().Equals(identifier))
                    {
                        m_commandLog.AddLog($"error: sector {sector.GetIdentifier()} already scanned", GameManager.RED);
                        return;
                    }
                }

                if (m_currentSectorScan != null && m_currentSectorScan.GetIdentifier().Equals(identifier))
                {
                    m_commandLog.AddLog($"error: sector {m_currentSectorScan.GetIdentifier()} already targeted for scan", GameManager.RED);
                    return;
                }

                if (m_scanCoroutine != null)
                    StopCoroutine(m_scanCoroutine);

                m_currentSectorScan = selectedSector;
                m_scanCoroutine = StartCoroutine(ScanSector(3));
            } 
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier}", GameManager.RED);
            }
        }));

        commandSystem.AddCommand(new CommandDefinition<Action<string>>("send", (string identifier) =>
        {
            ExpSector selectedSector = m_region.GetSector(identifier);
            if (selectedSector != null)
            {
                // TODO : Send squads on sectors
                // TODO : Prevent multiple squads on sector

            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier}", GameManager.RED);
            }
        }));

        m_region = ExpRegion.Generate();
    }

    private IEnumerator ScanSector(float totalTime)
    {
        Debug.Log("Starting scan...");
        m_commandLog.AddLog($"scan: prospecting data on sector {m_currentSectorScan.GetIdentifier()}", GameManager.ORANGE);
        float time = 0;
        while (time < totalTime)
        {
            yield return null;
            time += Time.deltaTime;
        }

        Debug.Log($"Scan completed for sector {m_currentSectorScan.GetIdentifier()}...");
        m_commandLog.AddLog($"scan: completed sector {m_currentSectorScan.GetIdentifier()}", GameManager.ORANGE);
        
        m_scannedSectors.Add(m_currentSectorScan);
        m_currentSectorScan = null;
    }
}
