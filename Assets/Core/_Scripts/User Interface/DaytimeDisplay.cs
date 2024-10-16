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
        UpdateDaytimeText();
    }

    private void Update()
    {
        UpdateDaytimeText();
    }

    private void UpdateDaytimeText()
    {
        if (m_timeManager == null || m_daytimeTextField == null)
        {
            Debug.LogWarning("TimeManager or TextField is not assigned.");
            return;
        }

        string timeString = m_timeManager.MapTimeToString();
        string dayString = m_timeManager.MapDayToString();
        int currentDay = m_timeManager.GetCurrentDay() + 1;
        int currentWeek = m_timeManager.GetCurrentWeek();

        // Ensure values are within valid ranges
        if (string.IsNullOrEmpty(timeString) || string.IsNullOrEmpty(dayString) || currentDay < 1 || currentWeek < 1)
        {
            Debug.LogWarning("Invalid time data received from TimeManager.");
            return;
        }

        m_daytimeTextField.text = string.Format(STRING_FORMAT, timeString, dayString, currentDay, currentWeek);
    }
}
