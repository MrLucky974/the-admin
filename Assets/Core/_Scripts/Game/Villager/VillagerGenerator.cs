using UnityEngine;

public class VillagerGenerator
{
    private const string NAMES_LIST_FILE_PATH = "Names";
    private const string SURNAMES_LIST_FILE_PATH = "Surnames";

    private string[] m_names;
    private string[] m_surnames;

    public void Initialize()
    {
        // Load assets from Resources folder
        var namesList = Resources.Load<TextAsset>(NAMES_LIST_FILE_PATH);
        var surnamesList = Resources.Load<TextAsset>(SURNAMES_LIST_FILE_PATH);

        // Store the names in arrays
        m_names = namesList.text.Split('\n');
        m_surnames = surnamesList.text.Split('\n');
    }

    public string GenerateName()
    {
        //Give the full name and surname
        var rng = GameManager.RNG;

        string name = m_names.PickRandom(rng).Replace("\r", "");
        string surname = m_surnames.PickRandom(rng);
        string fullName = $"{name} {surname}";
        return fullName;
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

    public int SetPreciseAge(int min, int max)
    {
        int age = Random.Range(min, max);
        return age;
    }

    public int GetAgeDependingOnAgeStage(int ageStage)
    {
        if (ageStage == 1)
        {
            return SetPreciseAge(0, 8);
        }
        else if (ageStage == 2)
        {
            return SetPreciseAge(7, 15);
        }
        else
        {
            return SetPreciseAge(14, 21);
        }
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
