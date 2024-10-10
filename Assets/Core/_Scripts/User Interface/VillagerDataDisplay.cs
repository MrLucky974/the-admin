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
            var villager = villagerManager.GetVillagerByID(identifier);
            if (villager != null)
            {
                Display(villager);
            }
        }));
    }

    public void Display(VillagerData data)
    {
        if (data == null) return;

        m_identifierLabel.SetText(string.Format("{0} | {1}", data.GetID(), data.GetName()));
        m_genderLabel.SetText(string.Format("Gender: {0}", data.GetGender()));
        m_personalityLabel.SetText(string.Format("Personality: {0}", data.GetPersonality()));
        m_ageLabel.SetText(string.Format("Age: {0}", data.GetAgeStage()));
        m_healthLabel.SetText(string.Format("Health Status: {0}", data.GetHealthStatus()));
        m_workingStatusLabel.SetText(string.Format("Working Status: {0}", "CACA"));
        m_fatigueLabel.SetText(string.Format("Fatigue: {0}", JUtils.GenerateTextSlider(data.GetFatigue(),VillagerData.MIN_FATIGUE, VillagerData.MAX_FATIGUE, 5)));
    }
}
