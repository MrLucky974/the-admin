using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data class containing information for resource gathering gameplay elements.
/// </summary>
public class Squad
{
    public const int SQUAD_SIZE = 3;
    public const int DEFAULT_FORCE_POINTS = 30;

    public enum State
    {
        IDLE = 0,
        DEPARTURE,
        EXPLORATION,
        ARRIVAL,
        COMBAT,
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
        members.Print();

        var squad = new Squad(sector, members);
        return squad;
    }

    private readonly Sector m_sector;
    private readonly VillagerData[] m_members;

    private readonly int m_strength = DEFAULT_FORCE_POINTS;

    private readonly int m_enemyChance;

    private State m_state = State.DEPARTURE;
    private int m_timeSpent;
    private int m_maxTotalTime;
    private (ResourceType resourceType, int amount) m_resources;

    private Squad(Sector sector, VillagerData[] members)
    {
        if (sector == null)
        {
            throw new ArgumentNullException(nameof(sector), "Sector cannot be null.");
        }

        if (members == null)
        {
            throw new ArgumentNullException(nameof(members), "Members array cannot be null.");
        }

        foreach (var member in members)
        {
            if (member == null)
            {
                throw new ArgumentException("Members array contains null elements.", nameof(members));
            }
        }

        m_sector = sector;
        m_members = members;
        m_maxTotalTime = TimeManager.DAY_IN_SECONDS / 2;

        // Caculate force value used in combat to determine the outcome.
        m_strength = DEFAULT_FORCE_POINTS;
        foreach (var member in members)
        {
            // VillagerData.MAX_FATIGUE = 10
            int fatigue = member.GetFatigue();
            m_strength -= fatigue;
        }
        m_strength = Mathf.Max(m_strength, 0); // Ensure value cannot get under zero

        var rng = GameManager.RNG;
        m_enemyChance = rng.Next(10, 101);
    }

    public int GetStrength() { return m_strength; }

    private enum CombatIssue
    {
        TIE,
        SUCCESS,
        FAILURE,
        CRITICAL_FAILURE
    }

    private CombatIssue InitiateCombat(System.Random rng)
    {
        int enemyStrength = rng.Next(10, 40);
        int difference = m_strength - enemyStrength;

        int outcomeRoll = rng.Next(0, 100);
        if (difference > 10)
        {
            // Squad is strong compared to enemy
            if (outcomeRoll < 70) return CombatIssue.SUCCESS; // Higher chance of success
            if (outcomeRoll < 90) return CombatIssue.TIE;     // Moderate chance of tie
            return CombatIssue.FAILURE;                        // Low chance of failure
        }
        else if (difference > -10)
        {
            // Squad and enemy have similar strength
            if (outcomeRoll < 40) return CombatIssue.SUCCESS; // Moderate chance of success
            if (outcomeRoll < 80) return CombatIssue.TIE;     // Higher chance of tie
            return CombatIssue.FAILURE;                        // Low chance of failure
        }
        else
        {
            // Squad is weaker than enemy
            if (outcomeRoll < 20) return CombatIssue.SUCCESS; // Low chance of success
            if (outcomeRoll < 60) return CombatIssue.TIE;     // Moderate chance of tie
            if (outcomeRoll < 90) return CombatIssue.FAILURE; // Higher chance of failure
            return CombatIssue.CRITICAL_FAILURE;              // High chance of critical failure
        }
    }

    public void Process(NarratorSystem narrator, CommandLogManager commandLog)
    {
        var rng = GameManager.RNG;
        var resourceHandler = GameManager.Instance.GetResourceHandler();

        // TODO : Check for squad health status and update progression
        switch (m_state)
        {
            case State.DEPARTURE: // Going to the sector
                m_timeSpent++;

                if (m_timeSpent >= m_maxTotalTime)
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

                    m_timeSpent = 0;

                    int enemyChance = rng.Next(0, 101);
                    Debug.Log($"{enemyChance} | {m_enemyChance}");
                    if (enemyChance >= m_enemyChance)
                    {
                        Debug.Log("initiating combat");
                        commandLog.AddLog($"info: squad engaged combat with enemy", GameManager.ORANGE);
                        m_state = State.COMBAT;
                        break;
                    }

                    m_state = type == ResourceType.NONE ? State.ARRIVAL : State.EXPLORATION;

                    var data = new SquadStatusChangedEvent()
                    {
                        PreviousState = State.DEPARTURE,
                        CurrentState = m_state,
                        Squad = this,
                    };
                    narrator.TriggerEvent(ExplorationEvents.SQUAD_STATUS_CHANGED, data);
                }
                else
                {
                    if (m_timeSpent % (m_maxTotalTime / 2) == 0)
                    {
                        commandLog.AddLog($"info: squad travel status on sector {m_sector.GetIdentifier()} is at 50%", GameManager.ORANGE);
                        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                    }
                }
                break;
            case State.COMBAT:
                m_timeSpent++;

                if (m_timeSpent % 8 == 0)
                {
                    CombatIssue combatIssue = InitiateCombat(rng);
                    var members = new List<VillagerData>(m_members);
                    switch (combatIssue)
                    {
                        case CombatIssue.TIE:
                            commandLog.AddLog($"info: squad managed to fend off enemies, with little to no harm", GameManager.ORANGE);
                            m_state = State.EXPLORATION;
                            break;
                        case CombatIssue.SUCCESS:
                            commandLog.AddLog($"info: squad managed to kill off enemies", GameManager.ORANGE);
                            m_state = State.EXPLORATION;

                            JRandom.WeightSumGenerator<ResourceType> resourceGenerator = new();
                            resourceGenerator.Add(ResourceType.RATIONS, 10);
                            resourceGenerator.Add(ResourceType.MEDS, 10);
                            resourceGenerator.Add(ResourceType.SCRAPS, 90);

                            var resource = resourceGenerator.Generate();
                            var amount = rng.Next(1, 3);
                            switch (resource)
                            {
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
                            commandLog.AddLog($"info: enemies dropped {amount} {resource}!", GameManager.ORANGE);
                            break;
                        case CombatIssue.FAILURE:
                            commandLog.AddLogError($"info: squad got harmed during operation, initiating return mission");

                            // One or two random members of the squad will be injured
                            int harmCount = rng.Next(1, 3);
                            for (int i = 0; i < harmCount; i++)
                            {
                                var member = members.PickRandom(rng);
                                member.ApplyHealthStatus(VillagerData.HealthStatus.INJURED);
                                members.Remove(member);
                            }
                            m_state = State.ARRIVAL;
                            break;
                        case CombatIssue.CRITICAL_FAILURE:
                            commandLog.AddLogError($"info: squad suffered great losses on combat");

                            var villagerManager = GameManager.Instance.GetVillagerManager();
                            int deathCount = rng.Next(1, 4);
                            for (int i = 0; i < deathCount; i++)
                            {
                                var member = members.PickRandom(rng);
                                villagerManager.Kill(member, VillagerManager.DeathType.COMBAT);
                                members.Remove(member);
                            }

                            int remainingMembers = SQUAD_SIZE;
                            foreach (var member in m_members)
                            {
                                if (member.IsDead())
                                {
                                    remainingMembers -= 1;
                                }
                            }

                            if (remainingMembers <= 0)
                            {
                                commandLog.AddLogError($"info: squad on sector {m_sector.GetIdentifier()} was killed off");
                                m_state = State.IDLE;
                            }
                            else
                            {
                                commandLog.AddLog($"info: remaining members of squad on sector {m_sector.GetIdentifier()} is preparing their anticipated return mission", GameManager.ORANGE);
                                m_state = State.ARRIVAL;
                            }
                            break;
                    }
                    m_timeSpent = 0;
                }
                break;
            case State.EXPLORATION: // Gathering resources
                m_timeSpent++;
                if (m_timeSpent >= m_maxTotalTime)
                {
                    Debug.Log($"resources {m_sector.GetResourceData()} gathered by {this}");

                    commandLog.AddLog($"info: squad on sector {m_sector.GetIdentifier()} completed expedition, initiating return mission", GameManager.ORANGE);
                    SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                    m_resources = m_sector.GetResourceData();
                    m_sector.Loot();

                    m_timeSpent = 0;
                    m_state = State.ARRIVAL;

                    var data = new SquadStatusChangedEvent()
                    {
                        PreviousState = State.DEPARTURE,
                        CurrentState = m_state,
                        Squad = this,
                    };
                    narrator.TriggerEvent(ExplorationEvents.SQUAD_STATUS_CHANGED, data);
                }
                else
                {
                    if (m_timeSpent % (m_maxTotalTime / 2) == 0)
                    {
                        commandLog.AddLog($"info: squad gathering status on sector {m_sector.GetIdentifier()} is at 50%", GameManager.ORANGE);
                        SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
                    }
                }
                break;
            case State.ARRIVAL: // Coming back from the sector
                m_timeSpent++;
                if (m_timeSpent >= m_maxTotalTime)
                {
                    Debug.Log($"squad {this} completed mission");
                    commandLog.AddLog($"info: squad from sector {m_sector.GetIdentifier()} is back to base", GameManager.ORANGE);
                    SoundManager.PlaySound(SoundType.ACTION_CONFIRM);

                    // Add resources
                    switch (m_resources.resourceType)
                    {
                        case ResourceType.RATIONS:
                            resourceHandler.AddRations(m_resources.amount);
                            break;
                        case ResourceType.MEDS:
                            resourceHandler.AddMeds(m_resources.amount);
                            break;
                        case ResourceType.SCRAPS:
                            resourceHandler.AddScraps(m_resources.amount);
                            break;
                    }

                    m_timeSpent = 0;
                    m_state = State.IDLE;

                    var data = new SquadStatusChangedEvent()
                    {
                        PreviousState = State.DEPARTURE,
                        CurrentState = m_state,
                        Squad = this,
                    };
                    narrator.TriggerEvent(ExplorationEvents.SQUAD_STATUS_CHANGED, data);
                }
                else
                {
                    if (m_timeSpent % (m_maxTotalTime / 2) == 0)
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
    public int GetTimeSpent() => m_timeSpent;
    public float GetProgress() => (float)m_timeSpent / m_maxTotalTime;

    public override string ToString()
    {
        return $"Squad({m_sector.GetIdentifier()} | State = {m_state} ({Mathf.RoundToInt(((float)m_timeSpent / TimeManager.DAY_IN_SECONDS) * 100)}%))";
    }
}
