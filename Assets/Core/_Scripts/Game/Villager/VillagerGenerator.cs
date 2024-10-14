using System;
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

    /// <summary>
    /// Generates a random name for a villager based on the surnames of two parent villagers.
    /// </summary>
    /// <param name="parent1">The first parent villager's data. Must not be null.</param>
    /// <param name="parent2">The second parent villager's data. Must not be null.</param>
    /// <returns>A string representing the generated full name of the villager, composed of a random first name and a random surname derived from the parents.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either <paramref name="parent1"/> or <paramref name="parent2"/> is null.</exception>
    public string GenerateNameFromParents(VillagerData parent1, VillagerData parent2)
    {
        if (parent1 == null)
            throw new ArgumentNullException(nameof(parent1));

        if (parent2 == null)
            throw new ArgumentNullException(nameof(parent2));

        var surname1 = parent1.GetName().Split(' ')[1];
        var surname2 = parent2.GetName().Split(' ')[1];
        var surnames = new string[] { surname1, surname2 };

        var rng = GameManager.RNG;
        string name = m_names.PickRandom(rng);
        string surname = surnames.PickRandom(rng);

        string fullName = $"{name} {surname}";
        return fullName.Replace("\r", "");
    }

    public string GenerateName()
    {
        //Give the full name and surname
        var rng = GameManager.RNG;

        string name = m_names.PickRandom(rng);
        string surname = m_surnames.PickRandom(rng);
        string fullName = $"{name} {surname}";
        return fullName.Replace("\r", "");
    }

    public int GenerateAge()
    {
        var rng = GameManager.RNG;
        int age = rng.Next(AGE_RANGE[AgeStage.KID].min, 20);
        return age;
    }

    public int GenerateAge(int min, int max)
    {
        var rng = GameManager.RNG;
        int age = rng.Next(min, max + 1);
        return age;
    }

    public int GenerateAgeByStage(AgeStage ageStage)
    {
        switch (ageStage)
        {
            case AgeStage.KID:
                return GenerateAge(AGE_RANGE[AgeStage.KID].min, AGE_RANGE[AgeStage.KID].max);
            case AgeStage.ADULT:
                return GenerateAge(AGE_RANGE[AgeStage.ADULT].min, AGE_RANGE[AgeStage.ADULT].max);
            case AgeStage.ELDER:
                return GenerateAge(AGE_RANGE[AgeStage.ELDER].min, 20);
        }

        return DEFAULT_AGE;
    }

    public Gender SelectRandomGender()
    {
        var rng = GameManager.RNG;
        return ALL_GENDERS.PickRandom(rng);
    }

    public Personality SelectRandomPersonality()
    {
        var rng = GameManager.RNG;
        return ALL_PERSONALITIES.PickRandom(rng);
    }

    public string GenerateID(string name, int populationCount)
    {
        string ID = name[0] + populationCount.ToString();
        return ID;
    }
}
