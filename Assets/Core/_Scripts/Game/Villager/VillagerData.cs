using System;
using UnityEngine;

public class VillagerData
{
    public enum AgeStage
    {
        KID,
        ADULT,
        ELDER
    };

    public enum Gender
    {
        MALE,
        FEMALE
    };

    public static Gender[] ALL_GENDERS =
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
        PREGNANT = 1 << 3
    };

    public enum Personality
    {
        NORMAL,
        HARDWORKER,
        LAZY,
        UNSTABLE
    }

    public static Personality[] ALL_PERSONALITIES =
    {
        Personality.NORMAL,
        Personality.HARDWORKER,
        Personality.LAZY,
        Personality.UNSTABLE
    };

    public enum WorkingStatus
    {
        ON_BASE,
        ON_EXPEDITION,
        WORKING_ON_BASE,
        RESTING
    }

    #region Default Values
    private const int DEFAULT_AGE = 7;
    private const Gender DEFAULT_GENDER = Gender.MALE;
    private const Personality DEFAULT_PERSONALITY = Personality.NORMAL;
    #endregion

    public const int MIN_FATIGUE = 0;
    public const int MAX_FATIGUE = 10;
    public const int DEFAULT_RECOVERY_SPEED = 5;

    public const int DEFAULT_WORKING_SPEED = 5;

    string m_identifier = "None";
    string m_name;
    int m_age = DEFAULT_AGE;
    int m_fatigue = 0;
    int m_hunger = 0;



    RoomData m_currentRoom;

    Gender m_gender = DEFAULT_GENDER;
    HealthStatus m_healthStatus = HealthStatus.HEALTHY;
    WorkingStatus m_workingStatus;
    Personality m_personality = DEFAULT_PERSONALITY;
    AgeStage m_ageStage;

    public VillagerData(string name)
    {
        m_name = name;
    }

    public void UpdateAgeStatus()
    {
        m_age += 1;

        if (m_age >= 0 && m_age <= 7)
        {
            m_ageStage = AgeStage.KID;
        }
        else if (m_age > 7 && m_age <= 14)
        {
            m_ageStage = AgeStage.ADULT;
        }
        else
        {
            m_ageStage = AgeStage.ELDER;
            if (m_age >= 21)
            {
                //the vilager will die
            }
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

    public Gender GetGender()
    {
        return m_gender;
    }

    public Personality GetPersonality()
    {
        return m_personality;
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

    #region Handling Fatigue
    public void DecreaseFatigue(int fatigueToRemove)
    {
        m_fatigue = m_fatigue - Mathf.Abs(fatigueToRemove);
        Mathf.Clamp(m_fatigue, MIN_FATIGUE, MAX_FATIGUE);
    }

    public void IncreaseFatigue(int fatigueToAdd)
    {
        m_fatigue = m_fatigue + Mathf.Abs(fatigueToAdd);
        Mathf.Clamp(m_fatigue, MIN_FATIGUE, MAX_FATIGUE);
    }

    public int GetFatigue()
    {
        return m_fatigue;
    }
    #endregion

    public void Impregnate()
    {
        RemoveHealthStatus(HealthStatus.HEALTHY);
        ApplyHealthStatus(HealthStatus.PREGNANT);
    }

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
    public HealthStatus GetHealthStatus()
    {
        return m_healthStatus;
    }
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

