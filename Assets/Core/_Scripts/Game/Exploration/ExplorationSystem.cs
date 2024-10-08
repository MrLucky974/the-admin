using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : Take care of the backlog of the squad expedition system
public class ExplorationSystem : MonoBehaviour
{
    private CommandLogManager m_commandLog;

    private ExpRegion m_region;

    // Scanning
    private ExpSector m_currentSectorScan;
    private Coroutine m_scanCoroutine;

    private List<ExpSector> m_scannedSectors;

    // Exploration
    private List<ExpSquad> m_squads;

    public void Initialize()
    {
        m_scannedSectors = new List<ExpSector>();
        m_squads = new List<ExpSquad>();

        var commandSystem = GameManager.Instance.GetCommands();
        m_commandLog = GameManager.Instance.GetCommandLog();

        // This command generates a new region (grid) of sectors (cells) for exploration
        commandSystem.AddCommand(new CommandDefinition<Action>("region", () =>
        {
            // TODO : Open modal box when region is not explored at 100%
            m_region = ExpRegion.Generate();
        }));

        // This command starts a scanning operation that will give information to the player
        // about resources on the specified sector
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("scan", (string identifier) =>
        {
            ExpSector selectedSector = m_region.GetSector(identifier.ToUpper());
            if (selectedSector != null)
            {
                foreach (var sector in m_scannedSectors)
                {
                    if (sector.GetIdentifier().Equals(identifier.ToUpper()))
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

                // Stop the sector being currently scanned
                // TODO : Open modal box to confirm this action
                if (m_scanCoroutine != null)
                    StopCoroutine(m_scanCoroutine);

                // TODO : Add the ability for multiple scans (e.g. satellite upgrades)
                // Begin coroutine and mark sector as being scanned
                m_currentSectorScan = selectedSector;
                m_scanCoroutine = StartCoroutine(ScanSector(3));
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
            }
        }));

        // This command launch an expedition to the specified sector, with a probability
        // of coming back with resources
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("send", (string identifier) =>
        {
            ExpSector selectedSector = m_region.GetSector(identifier.ToUpper());
            if (selectedSector != null)
            {
                // Check if a squad is already exploring the selected sector
                foreach (var squad in m_squads)
                {
                    var (sector, _) = squad.GetExplorationStatus();
                    if (sector.GetIdentifier().Equals(identifier.ToUpper()))
                    {
                        m_commandLog.AddLog($"error: sector {identifier.ToUpper()} already being explored by another squad", GameManager.RED);
                        return;
                    }
                }

                // Check if the selected sector has been scanned and nothing was found
                foreach (var sector in m_scannedSectors)
                {
                    if (sector.GetIdentifier().Equals(identifier.ToUpper()))
                    {
                        var resourceData = sector.GetResourceData();
                        if (resourceData.resourceType == ResourceType.NONE)
                        {
                            m_commandLog.AddLog($"error: sector {sector.GetIdentifier()} is empty", GameManager.RED);
                            return;
                        }
                        break;
                    }
                }

                // Check if the selected sector has already been looted
                foreach (var sector in m_region.GetSectors())
                {
                    if (sector.GetIdentifier().Equals(identifier) && sector.IsLooted())
                    {
                        m_commandLog.AddLog($"error: sector {sector.GetIdentifier()} has already been looted", GameManager.RED);
                        return;
                    }
                }

                // Send the squad into expedition
                // TODO : Add delay before squad arrival
                ExpSquad newSquad = ExpSquad.Generate(selectedSector);
                m_squads.Add(newSquad);
                m_commandLog.AddLog($"send: squad sent to sector {identifier.ToUpper()}", GameManager.ORANGE);
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
            }
        }));

        // Generate a starting region on game start
        m_region = ExpRegion.Generate();
    }

    /// <summary>
    /// Coroutine that takes care of the scan operation, which is just a delay before displaying
    /// data to the player on the command log.
    /// </summary>
    /// <param name="totalTime">The scan "delay" in seconds</param>
    /// <returns></returns>
    private IEnumerator ScanSector(float totalTime)
    {
        Debug.Log("Starting scan...");
        m_commandLog.AddLog($"scan: prospecting data on sector {m_currentSectorScan.GetIdentifier()}", GameManager.ORANGE);
        
        // Wait until the time value has reached the total time
        // Could use WaitForSeconds but me no likey
        float time = 0;
        while (time < totalTime)
        {
            yield return null;
            time += Time.deltaTime;
        }

        // Send a message on the command log to signal the player a scan was completed
        // and display the result
        Debug.Log($"Scan completed for sector {m_currentSectorScan.GetIdentifier()}...");
        m_commandLog.AddLog($"scan: completed sector {m_currentSectorScan.GetIdentifier()}", GameManager.ORANGE);

        var resourceData = m_currentSectorScan.GetResourceData();
        if (resourceData.resourceType == ResourceType.NONE)
        {
            m_commandLog.AddLog("Found nothing.", GameManager.ORANGE);
        }
        else
        {
            m_commandLog.AddLog($"Found {resourceData.amount} {resourceData.resourceType}!", GameManager.ORANGE);
        }

        // Mark the sector as scanned
        m_scannedSectors.Add(m_currentSectorScan);
        m_currentSectorScan = null;
    }
}
