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
    private Queue<ExpSquad> m_completeSquadsQueue;

    public void Initialize()
    {
        m_scannedSectors = new List<ExpSector>();
        m_squads = new List<ExpSquad>();
        m_completeSquadsQueue = new Queue<ExpSquad>();

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

                if (m_scanCoroutine != null)
                    StopCoroutine(m_scanCoroutine);

                m_currentSectorScan = selectedSector;
                m_scanCoroutine = StartCoroutine(ScanSector(3));
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
            }
        }));

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
                int squadIndex = m_squads.Count;
                ExpSquad newSquad = new ExpSquad(squadIndex, selectedSector);
                m_squads.Add(newSquad);
                m_commandLog.AddLog($"send: squad {squadIndex} sent to sector {identifier.ToUpper()}", GameManager.ORANGE);
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
            }
        }));

        m_region = ExpRegion.Generate();
    }

    public void UpdateSquads()
    {
        // TODO : Add delay before squad return
        // Remove complete squads
        ExpSquad toDequeue;
        while ((toDequeue = m_completeSquadsQueue.Dequeue()) != null)
        {
            if (m_squads.Contains(toDequeue))
            {
                m_squads.Remove(toDequeue);
            }
        }

        // Update each in-going squad
        foreach (var squad in m_squads)
        {
            var status = squad.Update();
            if (status == ExpSquad.Status.DONE)
            {
                m_completeSquadsQueue.Enqueue(squad);
                m_commandLog.AddLog($"send: squad {squad.GetIdentifier()} completed expedition, initiating return operation", GameManager.ORANGE);
            }
        }
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

        var resourceData = m_currentSectorScan.GetResourceData();
        if (resourceData.resourceType == ResourceType.NONE)
        {
            m_commandLog.AddLog("Found nothing.", GameManager.ORANGE);
        }
        else
        {
            m_commandLog.AddLog($"Found {resourceData.amount} {resourceData.resourceType}!", GameManager.ORANGE);
        }

        m_scannedSectors.Add(m_currentSectorScan);
        m_currentSectorScan = null;
    }
}
