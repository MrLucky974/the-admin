using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using static System.Math;
public class VillagerData
{
    string Identifiant = "None";
    string Name;
    int Age;
    int Fatigue = 0;
    int RecoverySpeed = 5;
    int Hunger = 0;
    const int workingSpeed = 5;
    public enum AgeStage { KID, ADULT, ELDER };
    public enum Genre { MALE, FEMALE };

    [Flags] public enum HealthStatus { 
        HEALTHY = 0, 
        SICK = 1 << 0,
        INJURED = 1 << 1,
        HUNGRY = 1 << 2,
        PREGNANT = 1 << 3
    };
    public enum Personality { NORMAL, HARDWORKER,  LAZY, UNSTABLE }
    public enum WorkingStatus { ONBASE, ONEXPEDITION, WORKINGONBASE, RESTING}


    Genre genre;
    HealthStatus healthStatus;
    Personality personality;
    AgeStage ageStage;

    public void UpdateAgeStatus()
    {
        Age += 1;

        if (Age >= 0 && Age <= 7)
        {
            ageStage = AgeStage.KID;
        }
        else if (Age > 7 && Age <= 14)
        {
            ageStage = AgeStage.ADULT;
        }
        else
        {
            ageStage = AgeStage.ELDER;
            if (Age >= 21)
            {
                //the vilager will die
            }
        }

    }
    public AgeStage CheckAgeStage()
    {
        AgeStage currentAgeSatge = ageStage;
        return currentAgeSatge;
    }

    public bool IsChild()
    {
        return ageStage == AgeStage.KID;
    }

    public bool IsAdult()
    {
        return ageStage == AgeStage.ADULT;
    }

    public bool IsElder()
    {
        return ageStage == AgeStage.ELDER;
    }

    public AgeStage GetAgeStage()
    {
        return ageStage;
    }
    public void UpdateName(string newName)
    {
        Name = newName;
    }
    public void NewAge(int newAge)
    {
        Age = newAge;
    }
    public void SetGender(int gender)
    {
        if (gender == 1)
        {
            genre = Genre.MALE;
        }
        else
        {
            genre = Genre.FEMALE;
        }
    }
    public Genre GetGender()
    {
        return genre;
    }
    public string GetName()
    {
        return Name;
    }
    public void SetID(string newID)
    {
        Identifiant = newID;
    }

    public string GetID()
    {
        return Identifiant;
    }

    public void SetPersonality (Personality typeToSet)
    {
        personality = typeToSet;
    }

    public Personality GetPersonality()
    {
        return personality;
    }

    public void DecreaseFatigue(int fatigueToRemove)
    {
        Fatigue = Fatigue - fatigueToRemove;
        Mathf.Clamp(Fatigue, 0, 10);
    }
    public void IncreaseFatigue(int fatigueToAdd)
    {
        Fatigue = Fatigue + fatigueToAdd;
        Mathf.Clamp(Fatigue, 0, 10);
    }
    public int GetFatigue()
    {
        return Fatigue;
    }
    public void AddHealthStatus(HealthStatus statusToSet)
    {
        healthStatus |= statusToSet;
    }

    public bool HasHealthStatus(HealthStatus status)
    {
        return (healthStatus & status) == status;
    }

    // if (villager.HasHealthStatus(HealthStatus.PREGNANT)) {
    // ...
    // }
    public int GetIDLastDigit()
    {
        int lastdigit = Identifiant[1];
        return lastdigit;
    }
    public override string ToString()
    {
        return $"ID : {Identifiant} / {Name}(Age: {Age}, {ageStage}), Gender : {genre}, Personality : {personality}, HealthSt" +
            $"atus {healthStatus}";
    }
}

