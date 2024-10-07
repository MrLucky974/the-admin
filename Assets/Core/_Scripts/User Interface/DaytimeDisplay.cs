using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DaytimeDisplay : MonoBehaviour
{
    private const string STRING_FORMAT = "[{0}] | {1} (Day {2}) | Week {3}";

    [SerializeField] private TMP_Text m_daytimeTextField;
    private TimeManager m_timeManager;

    private void Start()
    {
        m_timeManager = GameManager.Instance.GetTimeManager();
        m_daytimeTextField.text = string.Format(STRING_FORMAT, m_timeManager.MapTimeToString(), m_timeManager.MapDayToString(), m_timeManager.GetCurrentDay() + 1, m_timeManager.GetCurrentWeek());
    }

    private void Update()
    {
        m_daytimeTextField.text = string.Format(STRING_FORMAT, m_timeManager.MapTimeToString(), m_timeManager.MapDayToString(), m_timeManager.GetCurrentDay() + 1, m_timeManager.GetCurrentWeek());
    }
}
