using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationSystem : MonoBehaviour
{
    public struct SectorEventData
    {
        public int X, Y;
        public float Progress;
        public (ResourceType resourceType, int amount) Resource;
    }

    public struct SquadStatusEventData
    {
        public int x, y;
        public bool finishedExploration;
        public float progress;
        public bool isSectorScanned;
        public (ResourceType resourceType, int amount) resource;
    }

    private CommandLogManager m_commandLog;
    private NarratorSystem m_narrator;
    private TimeManager m_timeManager;

    private Region m_region;

    // Scanning properties
    private bool m_updateScanProgress = true;
    private Sector m_currentSectorScan;
    private Coroutine m_scanCoroutine;

    private List<Sector> m_scannedSectors; // List of every sector already scanned on this current region
    public event Action<SectorEventData> OnSectorScanned;
    public event Action<SectorEventData> OnCurrentSectorScan;
    public event Action OnRegionRegeneration;
    public event Action<SquadStatusEventData> OnSquadStatusChanged;

    // Exploration properties
    private Coroutine m_explorationLoopCoroutine;
    private List<Squad> m_activeSquads;

    public void Initialize()
    {
        m_narrator = GameManager.Instance.GetNarrator();
        m_narrator.Subscribe<SquadStatusChangedEvent>(ExplorationEvents.SQUAD_STATUS_CHANGED, HandleSquadStatusChanged);

        m_timeManager = GameManager.Instance.GetTimeManager();
        //m_timeManager.OnDayEnded += UpdateMap;

        m_scannedSectors = new List<Sector>();
        m_activeSquads = new List<Squad>();
        m_explorationLoopCoroutine = StartCoroutine(nameof(Tick));

        #region Initialize Commands

        var commandSystem = GameManager.Instance.GetCommands();
        var modalBox = GameManager.Instance.GetModal();
        m_commandLog = GameManager.Instance.GetCommandLog();

        // This command generates a new region (grid) of sectors (cells) for exploration
        commandSystem.AddCommand(new CommandDefinition<Action>("region", "Scan for a new region to regenerate resources", () =>
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
                OnRegionRegeneration?.Invoke();

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
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("scan", "Scan a sector to unveil their resources and potential threats", (string identifier) =>
        {
            Sector selectedSector = m_region.GetSector(identifier.ToUpper());
            if (selectedSector != null)
            {
                // Check if sector identifiers matches and prevent it from being
                // scanned twice if true
                if (m_currentSectorScan != null && m_currentSectorScan.GetIdentifier().Equals(identifier))
                {
                    m_commandLog.AddLogError($"error: sector {m_currentSectorScan.GetIdentifier()} already targeted for scan");
                    return;
                }

                // Stop the sector being currently scanned
                if (m_scanCoroutine != null)
                {
                    m_updateScanProgress = false;
                    modalBox.Init("CANCEL PREVIOUS SCAN", (modal) =>
                    {
                        StopCoroutine(m_scanCoroutine);

                        // TODO : Add the ability for multiple scans (e.g. satellite upgrades)
                        // Begin coroutine and mark sector as being scanned
                        m_currentSectorScan = selectedSector;
                        m_scanCoroutine = StartCoroutine(ScanSector(TimeManager.DAY_IN_SECONDS / 2));
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
                    m_scanCoroutine = StartCoroutine(ScanSector(TimeManager.DAY_IN_SECONDS / 2));
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
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("send", "Send a squad on the sector to gather resources", (string identifier) =>
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
                        m_commandLog.AddLogError($"error: sector {identifier.ToUpper()} already being explored by another squad");
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
                            m_commandLog.AddLogError($"error: sector {sector.GetIdentifier()} is empty");
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
                        m_commandLog.AddLogError($"error: sector {sector.GetIdentifier()} has already been looted!");
                        return;
                    }
                }

                var resourceHandler = GameManager.Instance.GetResourceHandler();
                if (resourceHandler.HasEnoughResources(Squad.SQUAD_SIZE, 0, 0) is false)
                {
                    m_commandLog.AddLogError($"error: not enough rations to send a squad (need {Squad.SQUAD_SIZE})!");
                    return;
                }

                // Send the squad into expedition
                Squad newSquad = Squad.Create(selectedSector);
                if (newSquad == null)
                {
                    m_commandLog.AddLogError($"error: not enough population to send a squad (need {Squad.SQUAD_SIZE})!");
                    return;
                }

                resourceHandler.ConsumeRations(Squad.SQUAD_SIZE);
                m_activeSquads.Add(newSquad);
                m_commandLog.AddLog($"send: squad sent to sector {identifier.ToUpper()} (estimated strength: {newSquad.GetStrength()})", GameManager.ORANGE);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            }
            else
            {
                m_commandLog.AddLogError($"error: invalid sector {identifier.ToUpper()}");
            }
        }));

        #endregion

        // Generate a starting region on game start
        m_region = Region.Generate();
        OnRegionRegeneration?.Invoke();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void HandleSquadStatusChanged(SquadStatusChangedEvent @event)
    {
        var squad = @event.Squad;
        var squadSector = squad.GetActivitySector();
        var coords = m_region.GetSectorCoordinates(squadSector);

        var data = new SquadStatusEventData()
        {
            x = coords.x,
            y = coords.y,
            finishedExploration = squad.GetState() == Squad.State.IDLE,
            progress = squad.GetProgress(),
            isSectorScanned = m_scannedSectors.Contains(squadSector),
            resource = squadSector.GetResourceData(),
        };
        OnSquadStatusChanged?.Invoke(data);
    }

    private IEnumerator Tick()
    {
        const float TICK_UPDATE = 1f;

        float elapsedTime = 0f;
        while (true)
        {
            // Remove all inactive squads from the update list
            m_activeSquads.RemoveAll((squad) => squad.GetState() == Squad.State.IDLE);

            if (elapsedTime < TICK_UPDATE)
            {
                yield return null;
                elapsedTime += Time.deltaTime * m_timeManager.GetTimeScale();
                continue;
            }

            elapsedTime = 0f;
            foreach (var squad in m_activeSquads)
            {
                squad.Process(m_narrator, m_commandLog);

                if (squad.GetState() != Squad.State.IDLE)
                {
                    var sector = squad.GetActivitySector();
                    var coords = m_region.GetSectorCoordinates(sector);

                    var data = new SquadStatusEventData()
                    {
                        x = coords.x,
                        y = coords.y,
                        progress = squad.GetProgress(),
                        isSectorScanned = m_scannedSectors.Contains(sector),
                        resource = sector.GetResourceData(),
                    };
                    OnSquadStatusChanged?.Invoke(data);
                }
                else
                {
                    var reputationHandler = GameManager.Instance.GetReputationHandler();
                    var members = squad.GetMembers();

                    int remainingMembers = Squad.SQUAD_SIZE;
                    foreach (var member in members)
                    {
                        if (member.IsDead())
                        {
                            remainingMembers--;
                            continue;
                        }
                    }

                    reputationHandler.IncreaseReputation(remainingMembers * 2);
                }
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

        var coords = m_region.GetSectorCoordinates(m_currentSectorScan);

        // Wait until the time value has reached the total time
        // Could use WaitForSeconds but me no likey
        float time = 0;
        while (time < totalTime)
        {
            yield return null;
            if (m_updateScanProgress) time += Time.deltaTime * m_timeManager.GetTimeScale();

            var sectorEventData = new SectorEventData()
            {
                Progress = time / totalTime,
                X = coords.x,
                Y = coords.y,
                Resource = m_currentSectorScan.GetResourceData(),
            };
            OnCurrentSectorScan?.Invoke(sectorEventData);
        }

        // Send a message on the command log to signal the player a scan was completed
        // and display the result
        Debug.Log($"Scan completed for sector {m_currentSectorScan.GetIdentifier()}...");
        m_commandLog.AddLog($"scan: completed sector {m_currentSectorScan.GetIdentifier()}", GameManager.ORANGE);

        var resourceData = m_currentSectorScan.GetResourceData();
        if (resourceData.resourceType == ResourceType.NONE)
        {
            m_commandLog.AddLog("Found nothing.", GameManager.ORANGE, false);
        }
        else
        {
            m_commandLog.AddLog($"Found {resourceData.amount} {resourceData.resourceType}!", GameManager.ORANGE, false);
        }

        // Mark the sector as scanned
        var data = new SectorEventData()
        {
            X = coords.x,
            Y = coords.y,
            Resource = resourceData,
        };
        OnSectorScanned?.Invoke(data);
        m_scannedSectors.Add(m_currentSectorScan);
        m_currentSectorScan = null;
        m_scanCoroutine = null;
    }
}
