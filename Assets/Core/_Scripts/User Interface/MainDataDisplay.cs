using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainDataDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_reputationLabel;

    [Space]

    [SerializeField] private TMP_Text m_totalPopulationLabel;
    [SerializeField] private TMP_Text m_childrenPopulationLabel;
    [SerializeField] private TMP_Text m_adultPopulationLabel;
    [SerializeField] private TMP_Text m_elderPopulationLabel;

    private VillagerManager m_villagerManager;
    private ReputationHandler m_reputationHandler;

    private void Awake()
    {
        m_villagerManager = GameManager.Instance.GetVillagerManager();
        m_villagerManager.OnPopulationChanged += UpdatePopulationValues;

        m_reputationHandler = GameManager.Instance.GetReputationHandler();
        m_reputationHandler.OnReputationChanged += UpdateReputationValue;
    }

    private void OnDestroy()
    {
        m_villagerManager.OnPopulationChanged -= UpdatePopulationValues;
        m_reputationHandler.OnReputationChanged -= UpdateReputationValue;
    }

    private void UpdateReputationValue(int value)
    {
        var sliderText = JUtils.GenerateTextSlider(value, 0, ReputationHandler.MAX_REPUTATION, 20);
        Color color = (value < 0) ? GameManager.RED : ((value == 0) ? GameManager.ORANGE : GameManager.GREEN);
        m_reputationLabel.SetText(JUtils.FormatColor($"Reputation: {sliderText} ({value})", color));
    }

    private void UpdatePopulationValues(List<VillagerData> list)
    {
        int total = list.Count;
        int availableWorkers = list.Count(data => data.IsIdle() && !data.IsChild());
        int totalWorkers = list.Count(data => !data.IsChild());

        int children = list.Count(data => data.IsChild());
        //int availableChildren = list.Count(data => data.IsIdle() && data.IsChild());

        int adults = list.Count(data => data.IsAdult());
        int availableAdults = list.Count(data => data.IsIdle() && data.IsAdult());

        int elder = list.Count(data => data.IsElder());
        int availableElder = list.Count(data => data.IsIdle() && data.IsElder());

        m_totalPopulationLabel.SetText($"Population: {availableWorkers} ({totalWorkers}) / {total}");
        m_childrenPopulationLabel.SetText($"Children: {children}");
        m_adultPopulationLabel.SetText($"Adults: {availableAdults} / {adults}");
        m_elderPopulationLabel.SetText($"Elder: {availableElder} / {elder}");
    }
}
