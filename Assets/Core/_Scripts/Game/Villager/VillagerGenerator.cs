using UnityEngine;
using static VillagerData;

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

    public int GenerateAge()
    {
        int age = Random.Range(AGE_RANGE[AgeStage.KID].min, 20);
        return age;
    }

    public int GenerateAge(int min, int max)
    {
        int age = Random.Range(min, max + 1);
        return age;
    }

    public int GenerateAgeByStage(VillagerData.AgeStage ageStage)
    {
        switch (ageStage) 
        {
            case VillagerData.AgeStage.KID:
                return GenerateAge(AGE_RANGE[AgeStage.KID].min, AGE_RANGE[AgeStage.KID].max);
            case VillagerData.AgeStage.ADULT:
                return GenerateAge(AGE_RANGE[AgeStage.ADULT].min, AGE_RANGE[AgeStage.ADULT].max);
            case VillagerData.AgeStage.ELDER:
                return GenerateAge(AGE_RANGE[AgeStage.ELDER].min, 20);
        }

        return VillagerData.DEFAULT_AGE;
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
