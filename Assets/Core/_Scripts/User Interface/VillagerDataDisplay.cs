using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VillagerDataDisplay : MonoBehaviour
{
    [SerializeField] private PageSwitcher m_pages;

    [Space]

    [SerializeField] private TMP_Text m_identifierLabel;
    [SerializeField] private TMP_Text m_genderLabel;
    [SerializeField] private TMP_Text m_personalityLabel;
    [SerializeField] private TMP_Text m_ageLabel;
    [SerializeField] private TMP_Text m_healthLabel;
    [SerializeField] private TMP_Text m_workingStatusLabel;
    [SerializeField] private TMP_Text m_fatigueLabel;

    private VillagerData m_lastData;

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
                m_pages.Select(StatusPageIndex.CHECKUP);
                Display(villager);
                SoundManager.PlaySound(SoundType.ACTION_CONFIRM);
            }
        }));

        villagerManager.OnPopulationChanged += UpdateDisplayData;
    }

    private void UpdateDisplayData(List<VillagerData> list)
    {
        // Only update the display if it is displayed on the screen
        if (m_pages.IsPageSelected() is false)
            return;

        if (m_pages.GetSelectedIndex<StatusPageIndex>().Equals(StatusPageIndex.CHECKUP) is false)
            return;

        if (list.Contains(m_lastData)) // Villager is present and data were probably modified
        {
            Display(m_lastData); // Update the values on the text
        }
        else // Villager was likely killed or died
        {
            m_lastData = null;
            m_pages.HideAll();
        }
    }

    public void Display(VillagerData data)
    {
        if (data == null) return;

        m_identifierLabel.SetText(string.Format("{0} | {1}", data.GetID(), data.GetName()));
        m_genderLabel.SetText(string.Format("Gender: {0}", data.GetGender()));
        m_personalityLabel.SetText(string.Format("Personality: {0}", data.GetPersonality()));
        m_ageLabel.SetText(string.Format("Age: {0}", data.GetAgeStage()));
        m_healthLabel.SetText(string.Format("Health Status: {0}", data.GetHealthStatus()));
        m_workingStatusLabel.SetText(string.Format("Working Status: {0}", data.GetWorkingStatus()));
        m_fatigueLabel.SetText(string.Format("Fatigue: {0}", data.GetFatigue()));

        m_lastData = data;
    }
}
