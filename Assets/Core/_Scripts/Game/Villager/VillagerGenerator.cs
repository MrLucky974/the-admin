using System.Collections;
using UnityEngine;
using System.IO;

public class VillagerGenerator
{
    // Start is called before the first frame update
    string[] _names;
    string[] _surnames;
    public string GetName()
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

    public int SetGender()
    {
        int genderset = Random.Range(1, 3);
        return genderset;
    }
    public string SetVillagerID(string firstLetterOfName, int PopulationNumber)
    {
        string ID = firstLetterOfName[0] + PopulationNumber.ToString();
        return ID;
    }
    public int SetPersonality()
    {
        int choosenPersonality = Random.Range(0, 4);
        return choosenPersonality;
    }
}
