using System;
using System.Collections.Generic;
using UnityEngine;

public class VillagerData
{
    public enum AgeStage
    {
        KID,
        ADULT,
        ELDER
    };

    public static readonly Dictionary<AgeStage, (int min, int max)> AGE_RANGE = new Dictionary<AgeStage, (int min, int max)>
    {
        { AgeStage.KID, (0, 7) },
        { AgeStage.ADULT, (8, 20) },
        { AgeStage.ELDER, (21, 29) },
    };

    public enum Gender
    {
        MALE,
        FEMALE
    };

    public static readonly Gender[] ALL_GENDERS =
    {
        Gender.MALE,
        Gender.FEMALE
    };

    [Flags]
    public enum HealthStatus
    {
        HEALTHY = 0,
        SICK = 1 << 0,
        INJURED = 1 << 1,
        HUNGRY = 1 << 2,
        PREGNANT = 1 << 3,
        STARVED = 1 << 4
    };

    public enum Personality
    {
        NORMAL,
        HARDWORKER,
        LAZY,
        UNSTABLE
    }

    public static readonly Personality[] ALL_PERSONALITIES =
    {
        Personality.NORMAL,
        Personality.HARDWORKER,
        Personality.LAZY,
        Personality.UNSTABLE
    };

    public enum WorkingStatus
    {
        IDLE,
        EXPEDITION,
        MAINTENANCE,
    }

    #region Default Values
    public const int DEFAULT_AGE = 8; // Default adult age
    public const Gender DEFAULT_GENDER = Gender.MALE;
    public const Personality DEFAULT_PERSONALITY = Personality.NORMAL;
    #endregion

    // Constants
    public const int MIN_FATIGUE = 0;
    public const int MAX_FATIGUE = 10;
    public const int DEFAULT_RECOVERY_VALUE = 1;

    public const int DEFAULT_WORKING_SPEED = 5;

    // Identity
    string m_identifier = "None";
    string m_name;
    int m_age = DEFAULT_AGE;
    AgeStage m_ageStage;
    Gender m_gender = DEFAULT_GENDER;
    Personality m_personality = DEFAULT_PERSONALITY;

    // Working Abilities
    int m_fatigue = 0;
    int m_recoveryValue = DEFAULT_RECOVERY_VALUE;
    WorkingStatus m_workingStatus = WorkingStatus.IDLE;
    RoomData m_currentRoom;

    // Health & Pregnancy
    HealthStatus m_healthStatus = HealthStatus.HEALTHY;
    private VillagerData m_mate;
    private int m_pregnancyDuration = 0;
    private bool m_isDead;

    public VillagerData(string name)
    {
        m_name = name;
        UpdateAgeStatus();
    }

    public string GetName()
    {
        return m_name;
    }

    public void SetID(string newID)
    {
        m_identifier = newID;
    }

    public string GetID()
    {
        return m_identifier;
    }

    public int GetIDLastDigit()
    {
        int lastdigit = m_identifier[1];
        return lastdigit;
    }

    #region Age Handling Methods

    public void UpdateAgeStatus()
    {
        if (m_age >= AGE_RANGE[AgeStage.KID].min && m_age <= AGE_RANGE[AgeStage.KID].max)
        {
            m_ageStage = AgeStage.KID;
        }
        else if (m_age >= AGE_RANGE[AgeStage.ADULT].min && m_age <= AGE_RANGE[AgeStage.ADULT].max)
        {
            m_ageStage = AgeStage.ADULT;
        }
        else
        {
            m_ageStage = AgeStage.ELDER;
        }
    }

    #region Age Checking
    public bool IsChild()
    {
        return m_ageStage == AgeStage.KID;
    }

    public bool IsAdult()
    {
        return m_ageStage == AgeStage.ADULT;
    }

    public bool IsElder()
    {
        return m_ageStage == AgeStage.ELDER;
    }
    #endregion

    public AgeStage GetAgeStage()
    {
        return m_ageStage;
    }

    public void SetAge(int babyAge)
    {
        m_age = babyAge;
        UpdateAgeStatus();
    }

    public int GetOlder()
    {
        m_age++;
        UpdateAgeStatus();
        return m_age;
    }

    #endregion

    public Gender GetGender()
    {
        return m_gender;
    }

    public bool IsMale()
    {
        return m_gender == Gender.MALE;
    }

    public Personality GetPersonality()
    {
        return m_personality;
    }

    #region Work Handling Methods

    public WorkingStatus GetWorkingStatus()
    {
        return m_workingStatus;
    }

    public void SetWorkingStatus(WorkingStatus status)
    {
        m_workingStatus = status;
    }

    public bool IsOnExpedition()
    {
        return m_workingStatus.Equals(WorkingStatus.EXPEDITION);
    }

    public bool IsIdle()
    {
        return m_workingStatus.Equals(WorkingStatus.IDLE);
    }

    #endregion

    #region Fatigue Handling Methods

    public void DecreaseFatigue(int fatigueToRemove)
    {
        m_fatigue = m_fatigue - Mathf.Abs(fatigueToRemove);
        m_fatigue = Mathf.Clamp(m_fatigue, MIN_FATIGUE, MAX_FATIGUE);
    }

    public void IncreaseFatigue(int fatigueToAdd)
    {
        m_fatigue = m_fatigue + Mathf.Abs(fatigueToAdd);
        m_fatigue = Mathf.Clamp(m_fatigue, MIN_FATIGUE, MAX_FATIGUE);
    }


    public int GetFatigue()
    {
        return m_fatigue;
    }

    #endregion

    #region Recovery Handling Methods

    public int GetRecoveryValue()
    {
        return m_recoveryValue;
    }

    public void IncreaseRecoveryValue(int valueToAdd)
    {
        m_recoveryValue += valueToAdd;
    }
    public void DecreaseRecoveryValue(int valueToAdd)
    {
        m_recoveryValue -= valueToAdd;
    }

    public void ResetRecoveryValue()
    {
        m_recoveryValue = DEFAULT_RECOVERY_VALUE;
    }

    #endregion

    #region Pregnancy Handling Methods

    public void Impregnate(VillagerData mate)
    {
        m_mate = mate;
        RemoveHealthStatus(HealthStatus.HEALTHY);
        ApplyHealthStatus(HealthStatus.PREGNANT);
        m_pregnancyDuration = 0;
    }

    public VillagerData GetMate()
    {
        return (m_mate);
    }

    public int GetPregnancyDuration()
    {
        return m_pregnancyDuration;
    }

    public bool IsPregnant()
    {
        return HasHealthStatus(HealthStatus.PREGNANT);
    }

    public void UpdatePregnancy()
    {
        m_pregnancyDuration++;
    }

    #endregion

    #region Health Handling Methods

    public void ApplyHealthStatus(HealthStatus status)
    {
        m_healthStatus |= status;
    }

    public void RemoveHealthStatus(HealthStatus status)
    {
        m_healthStatus &= ~status;
    }

    public bool HasHealthStatus(HealthStatus status)
    {
        // Example Code:
        // if (villager.HasHealthStatus(HealthStatus.PREGNANT)) {
        // ...
        // }

        return (m_healthStatus & status) == status;
    }

    public bool HasAnyHealthStatus(params HealthStatus[] status)
    {
        foreach (var item in status)
        {
            if (HasHealthStatus(item))
            {
                return true;
            }
        }

        return false;
    }

    public HealthStatus GetHealthStatus()
    {
        return m_healthStatus;
    }

    public void MarkAsDead()
    {
        m_isDead = true;
    }

    public bool IsDead() => m_isDead;

    #endregion

    public override string ToString()
    {
        return $"ID : {m_identifier} / {m_name}(Age: {m_age}, {m_ageStage}), Gender : {m_gender}, Personality : {m_personality}, HealthSt" +
            $"atus {m_healthStatus}";
    }

    public class Builder
    {
        private readonly VillagerData m_data;

        public Builder(string name)
        {
            m_data = new VillagerData(name);
        }

        public Builder SetAge(int age)
        {
            m_data.m_age = age;
            m_data.UpdateAgeStatus();
            return this;
        }

        public Builder SetPersonality(Personality personality)
        {
            m_data.m_personality = personality;
            return this;
        }

        public Builder SetGender(Gender gender)
        {
            m_data.m_gender = gender;
            return this;
        }

        public VillagerData Build()
        {
            return m_data;
        }
    }
}

