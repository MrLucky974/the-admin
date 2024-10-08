using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data class containing information for resource gathering gameplay elements.
/// </summary>
public class Squad
{
    public const int SQUAD_SIZE = 3;

    public enum State
    {
        IDLE = 0,
        DEPARTURE,
        EXPLORATION,
        ARRIVAL,
    }

    public static Squad Create(Sector sector)
    {
        var villagerManager = GameManager.Instance.GetVillagerManager();

        var population = new List<VillagerData>(villagerManager.GetPopulation());

        // Filter every unavailable villager "entity" out of being drafted into a squad
        population = population.Where(villager => villager.IsIdle() && !villager.IsChild()).ToList();
        if (population.Count < SQUAD_SIZE)
        {
            Debug.LogError($"Not enough villager available: has {population.Count}, need {SQUAD_SIZE}");
            return null;
        }

        population.Sort((villager1, villager2) =>
        {
            return villager1.GetAgeStage().CompareTo(villager2.GetAgeStage());
        });

        VillagerData[] members = new VillagerData[SQUAD_SIZE];
        for (int i = 0; i < SQUAD_SIZE; i++)
        {
            var member = members[i] = population[i];
            villagerManager.SetWorkingStatus(member, VillagerData.WorkingStatus.EXPEDITION);
        }

        var squad = new Squad(sector, members);
        return squad;
    }

    private readonly Sector m_sector;
    private readonly VillagerData[] m_members;
    private State m_state = State.DEPARTURE;
    private int m_progress;
    private (ResourceType resourceType, int amount) m_resources;

    private Squad(Sector sector, VillagerData[] members)
    {
        m_sector = sector;

        // TODO : Check for null elements inside array
        m_members = members;
    }

    public void Process(NarratorSystem narrator, CommandLogManager commandLog)
    {
        // TODO : Check for squad health status and update progression
        switch (m_state)
        {
            case State.DEPARTURE: // Going to the sector
                m_progress++;

                if (m_progress >= TimeManager.DAY_IN_SECONDS)
                {
                    Debug.Log($"squad {this} arrived on sector");

                    commandLog.AddLog($"info: squad arrived on sector {m_sector.GetIdentifier()}", GameManager.ORANGE);
                    SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                    (ResourceType type, _) = m_sector.GetResourceData();
                    if (type == ResourceType.NONE)
                    {
                        commandLog.AddLog($"info: no resources found on sector, initiating return mission", GameManager.RED);
                        SoundManager.PlaySound(SoundType.ERROR);
                    }

                    m_progress = 0;
                    m_state = type == ResourceType.NONE ? State.ARRIVAL : State.EXPLORATION;
                }
                else
                {
                    if (m_progress % (TimeManager.DAY_IN_SECONDS / 2) == 0)
                    {
                        commandLog.AddLog($"info: squad travel status on sector {m_sector.GetIdentifier()} is at 50%", GameManager.ORANGE);
                        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                    }
                }
                break;
            case State.EXPLORATION: // Gathering resources
                m_progress++;
                if (m_progress >= TimeManager.DAY_IN_SECONDS)
                {
                    Debug.Log($"resources gathered by {this}");

                    commandLog.AddLog($"info: squad on sector {m_sector.GetIdentifier()} completed expedition, initiating return mission", GameManager.ORANGE);
                    SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                    m_resources = m_sector.GetResourceData();
                    m_sector.Loot();

                    m_progress = 0;
                    m_state = State.ARRIVAL;
                }
                else
                {
                    if (m_progress % (TimeManager.DAY_IN_SECONDS / 2) == 0)
                    {
                        commandLog.AddLog($"info: squad gathering status on sector {m_sector.GetIdentifier()} is at 50%", GameManager.ORANGE);
                        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                    }
                }
                break;
            case State.ARRIVAL: // Coming back from the sector
                m_progress++;
                if (m_progress >= TimeManager.DAY_IN_SECONDS)
                {
                    Debug.Log($"squad {this} completed mission");
                    var data = new SquadArrivalEvent
                    {
                        resources = m_resources,
                        members = m_members,
                    };
                    narrator.TriggerEvent(ExplorationEvents.SQUAD_BACK_TO_BASE, data);

                    commandLog.AddLog($"info: squad from sector {m_sector.GetIdentifier()} is back to base", GameManager.ORANGE);
                    SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                    m_progress = 0;
                    m_state = State.IDLE;
                }
                else
                {
                    if (m_progress % (TimeManager.DAY_IN_SECONDS / 2) == 0)
                    {
                        commandLog.AddLog($"info: squad from {m_sector.GetIdentifier()} return mission is at 50%", GameManager.ORANGE);
                        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                    }
                }
                break;
            default:
                break;
        }

        //Debug.Log($"tick on {this}");
    }

    public Sector GetActivitySector() => m_sector;
    public State GetState() => m_state;
    public int GetProgress() => m_progress;

    public override string ToString()
    {
        return $"Squad({m_sector.GetIdentifier()} | State = {m_state} ({Mathf.RoundToInt(((float)m_progress / TimeManager.DAY_IN_SECONDS) * 100)}%))";
    }
}
