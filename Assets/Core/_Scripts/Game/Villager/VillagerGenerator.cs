using System.Collections;
using UnityEngine;
using System.IO;

public class VillagerGenerator
{
    string[] _names;
    string[] _surnames;
    
    public string GenerateName()
    {
        string fileName = "Assets/Core/_Scripts/Game/Villager/NamesLists/The_Administrator.Name_01.txt";
        string fileSurname = "Assets/Core/_Scripts/Game/Villager/NamesLists/The_Administrator.Name_02.txt";
        _names = File.ReadAllLines(fileName);
        _surnames = File.ReadAllLines(fileSurname);
        
        //Giving the full name and surname
        string nameToSet = _names[GetRandomID(_names.Length)] + " " + _surnames[GetRandomID(_surnames.Length)];
        return nameToSet;
    }

    int GetRandomID(int size)
    {
        int id = Random.Range(0, size);
        return id;
    }

    public int SetAge()
    {
        int age = Random.Range(0, 20);
        return age;
    }

    public VillagerData.Gender SelectRandomGender()
    {
        var rng = GameManager.RNG;
        return VillagerData.ALL_GENDERS.PickRandom(rng);
    }

    public VillagerData.Personality SelectRandomPersonality()
    {
        var rng = GameManager.RNG;
        return VillagerData.ALL_PERSONALITIES.PickRandom(rng);
    }

    public string GenerateID(string name, int populationCount)
    {
        string ID = name[0] + populationCount.ToString();
        return ID;
    }
}
