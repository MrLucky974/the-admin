using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainDataDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text m_totalPopulationLabel;
    [SerializeField] private TMP_Text m_childrenPopulationLabel;
    [SerializeField] private TMP_Text m_adultPopulationLabel;
    [SerializeField] private TMP_Text m_elderPopulationLabel;

    private VillagerManager m_villagerManager;

    private void Awake()
    {
        m_villagerManager = GameManager.Instance.GetVillagerManager();
        m_villagerManager.OnPopulationChanged += UpdateValues;
    }

    private void OnDestroy()
    {
        m_villagerManager.OnPopulationChanged -= UpdateValues;
    }

    private void UpdateValues(List<VillagerData> list)
    {
        int total = list.Count;
        int children = list.Count(data => data.IsChild());
        int adults = list.Count(data => data.IsAdult());
        int elder = list.Count(data => data.IsElder());

        m_totalPopulationLabel.SetText($"Population: {total}");
        m_childrenPopulationLabel.SetText($"Children: {children}");
        m_adultPopulationLabel.SetText($"Adults: {adults}");
        m_elderPopulationLabel.SetText($"Elder: {elder}");
    }
}
