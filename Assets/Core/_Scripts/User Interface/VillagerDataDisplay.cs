using System;
using TMPro;
using UnityEngine;

public class VillagerDataDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_identifierLabel;
    [SerializeField] private TMP_Text m_genderLabel;
    [SerializeField] private TMP_Text m_personalityLabel;
    [SerializeField] private TMP_Text m_ageLabel;
    [SerializeField] private TMP_Text m_healthLabel;
    [SerializeField] private TMP_Text m_workingStatusLabel;
    [SerializeField] private TMP_Text m_fatigueLabel;

    private void Start()
    {
        var villagerManager = GameManager.Instance.GetVillagerManager();
        var commandLog = GameManager.Instance.GetCommandLog();
        var commandSystem = GameManager.Instance.GetCommands();
        commandSystem.AddCommand(new CommandDefinition<Action<string>>("checkup", (string identifier) =>
        {
            var population = villagerManager.GetPopulation();
            foreach (VillagerData villager in population)
            {
                if (identifier.ToUpper() == villager.GetID())
                {
                    Display(villager);
                    break;
                }
            }
        }));
    }

    public void Display(VillagerData data)
    {
        if (data == null) return;

        m_identifierLabel.text = string.Format("{0} | {1}", data.GetID(), data.GetName());
        m_genderLabel.text = string.Format("Gender: {0}", data.GetGender());
        m_personalityLabel.text = string.Format("Personality: {0}", data.GetPersonality());
        m_ageLabel.text = string.Format("Age: {0}", data.GetAgeStage());
        m_healthLabel.text = string.Format("Health Status: {0}", "PIPI");
        m_workingStatusLabel.text = string.Format("Working Status: {0}", "CACA");
        m_fatigueLabel.text = string.Format("Fatigue: {0}", data.GetFatigue());
    }
}
