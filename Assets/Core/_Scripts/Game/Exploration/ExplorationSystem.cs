using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : Take care of the backlog of the squad expedition system
public class ExplorationSystem : MonoBehaviour
{
    private CommandLogManager m_commandLog;
    private NarratorSystem m_narrator;

    private Region m_region;

    // Scanning properties
    private bool m_updateScanProgress = true;
    private Sector m_currentSectorScan;
    private Coroutine m_scanCoroutine;

    private List<Sector> m_scannedSectors; // List of every sector already scanned on this current region

    // Exploration properties
    private Coroutine m_explorationLoopCoroutine;
    private List<Squad> m_activeSquads;

    public void Initialize()
    {
        m_narrator = GameManager.Instance.GetNarrator();
        m_narrator.Subscribe<SquadArrivalEvent>(ExplorationEvents.SQUAD_BACK_TO_BASE, OnSquadArrival);

        m_scannedSectors = new List<Sector>();
        m_activeSquads = new List<Squad>();
        m_explorationLoopCoroutine = StartCoroutine(nameof(Tick));

        #region Initialize Commands

        var commandSystem = GameManager.Instance.GetCommands();
        var modalBox = GameManager.Instance.GetModal();
        m_commandLog = GameManager.Instance.GetCommandLog();

        // This command generates a new region (grid) of sectors (cells) for exploration
        commandSystem.AddCommand(new CommandDefinition<Action>("region", () =>
        {
            // TODO : Check for on-going expeditions before authorizing region scan
            // TODO : Open modal box when region is not explored at 100%
            modalBox.Init("REGENERATE REGION", (modal) =>
            {
                // Clear scan sector list and stop any sector scan in progress
                m_scannedSectors.Clear();
                if (m_scanCoroutine != null)
                {
                    StopCoroutine(m_scanCoroutine);
                    m_currentSectorScan = null;
                }

                // Generate a new region
                m_region = Region.Generate();

                modal.Close();
            })
            .SetBody("Are you sure you want to scan for a new region?")
            .SetDismissAction((modal) =>
            {
                modal.Close();
            })
            .Open();
        }));

        // This command starts a scanning operation that will give information to the player
        // about resources on the specified sector
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("scan", (string identifier) =>
        {
            Sector selectedSector = m_region.GetSector(identifier.ToUpper());
            if (selectedSector != null)
            {
                foreach (var sector in m_scannedSectors)
                {
                    if (sector.GetIdentifier().Equals(identifier.ToUpper()))
                    {
                        m_commandLog.AddLog($"error: sector {sector.GetIdentifier()} already scanned", GameManager.RED);
                        SoundManager.PlaySound(SoundType.ERROR);
                        return;
                    }
                }

                if (m_currentSectorScan != null && m_currentSectorScan.GetIdentifier().Equals(identifier))
                {
                    m_commandLog.AddLog($"error: sector {m_currentSectorScan.GetIdentifier()} already targeted for scan", GameManager.RED);
                    SoundManager.PlaySound(SoundType.ERROR);
                    return;
                }

                // Stop the sector being currently scanned
                // TODO : Open modal box to confirm this action
                if (m_scanCoroutine != null)
                {
                    m_updateScanProgress = false;
                    modalBox.Init("CANCEL PREVIOUS SCAN", (modal) =>
                    {
                        StopCoroutine(m_scanCoroutine);

                        // TODO : Add the ability for multiple scans (e.g. satellite upgrades)
                        // Begin coroutine and mark sector as being scanned
                        m_currentSectorScan = selectedSector;
                        m_scanCoroutine = StartCoroutine(ScanSector(3));
                        m_updateScanProgress = true;

                        modal.Close();
                    })
                    .SetBody("Are you sure you want to scan another sector? The current progress will be lost.")
                    .SetDismissAction((modal) =>
                    {
                        m_updateScanProgress = true;
                        modal.Close();
                    })
                    .Open();
                }
                else
                {
                    // TODO : Add the ability for multiple scans (e.g. satellite upgrades)
                    // Begin coroutine and mark sector as being scanned
                    m_currentSectorScan = selectedSector;
                    m_scanCoroutine = StartCoroutine(ScanSector(3));
                    m_updateScanProgress = true;
                }
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
                SoundManager.PlaySound(SoundType.ERROR);
            }
        }));

        // This command launch an expedition to the specified sector, with a probability
        // of coming back with resources
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("send", (string identifier) =>
        {
            Sector selectedSector = m_region.GetSector(identifier.ToUpper());
            if (selectedSector != null)
            {
                // Check if a squad is already exploring the selected sector
                foreach (var squad in m_activeSquads)
                {
                    var sector = squad.GetActivitySector();
                    if (sector.GetIdentifier().Equals(identifier.ToUpper()))
                    {
                        m_commandLog.AddLog($"error: sector {identifier.ToUpper()} already being explored by another squad", GameManager.RED);
                        SoundManager.PlaySound(SoundType.ERROR);
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
                            SoundManager.PlaySound(SoundType.ERROR);
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
                        SoundManager.PlaySound(SoundType.ERROR);
                        return;
                    }
                }

                // Send the squad into expedition
                Squad newSquad = Squad.Create(selectedSector);
                if (newSquad == null)
                {
                    m_commandLog.AddLog($"error: not enough population to send a squad", GameManager.RED);
                    SoundManager.PlaySound(SoundType.ERROR);
                    return;
                }

                m_activeSquads.Add(newSquad);
                m_commandLog.AddLog($"send: squad sent to sector {identifier.ToUpper()}", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            }
            else
            {
                m_commandLog.AddLog($"error: invalid sector {identifier.ToUpper()}", GameManager.RED);
                SoundManager.PlaySound(SoundType.ERROR);
            }
        }));

        #endregion

        // Generate a starting region on game start
        m_region = Region.Generate();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnSquadArrival(SquadArrivalEvent @event)
    {
        var villagerManager = GameManager.Instance.GetVillagerManager();
        var resourceHandler = GameManager.Instance.GetResourceHandler();

        (ResourceType type, int amount) = @event.resources;
        switch (type)
        {
            case ResourceType.NONE:
                break;
            case ResourceType.RATIONS:
                resourceHandler.AddRations(amount);
                break;
            case ResourceType.MEDS:
                resourceHandler.AddMeds(amount);
                break;
            case ResourceType.SCRAPS:
                resourceHandler.AddScraps(amount);
                break;
        }

        foreach (var member in @event.members)
        {
            villagerManager.SetWorkingStatus(member, VillagerData.WorkingStatus.IDLE);
        }
    }

    private IEnumerator Tick()
    {
        const float TICK_UPDATE = 1f;

        float elapsedTime = 0f;
        while (true)
        {
            // Remove all inactive squads from the update list
            var inactiveSquads = m_activeSquads.FindAll((squad) =>
            {
                return squad.GetState() == Squad.State.IDLE;
            });
            m_activeSquads.RemoveAll((squad) => inactiveSquads.Contains(squad));

            if (elapsedTime < TICK_UPDATE)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
                continue;
            }

            elapsedTime = 0f;
            foreach (var squad in m_activeSquads)
            {
                squad.Process(m_narrator, m_commandLog);
            }
            yield return null;
        }
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
            if (m_updateScanProgress) time += Time.deltaTime;
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
        m_scanCoroutine = null;
    }
}
