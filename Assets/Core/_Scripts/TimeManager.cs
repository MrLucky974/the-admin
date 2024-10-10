using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public const int DAY_IN_SECONDS = 10;
    public const int WEEK_LENGTH_IN_DAYS = 7;
    public static readonly string[] WEEK_DAYS =
    {
        "Firdai",
        "Twodai",
        "Thürdai",
        "Fordai",
        "Fivdai",
        "Sindai",
        "Sevdai"
    };

    private float m_timePassed;
    private int m_currentDay;
    private int m_currentWeek;

    public event Action<int> OnDayEnded;
    public event Action<int> OnWeekEnded;

    public void Initialize()
    {
        m_timePassed = 0;
        m_currentDay = 0;
        m_currentWeek = 1;
    }

    public void UpdateTime(float deltaTime)
    {
        m_timePassed += deltaTime;
        if (m_timePassed > DAY_IN_SECONDS)
        {
            OnDayEnded?.Invoke(m_currentDay);
            m_currentDay++;
            if (m_currentDay >= WEEK_LENGTH_IN_DAYS)
            {
                OnWeekEnded?.Invoke(m_currentWeek);
                m_currentWeek++;
                m_currentDay = 0;
            }

            m_timePassed = 0;
        }
    }

    public string MapTimeToString()
    {
        // Calculate the normalized time (0 to 1) representing progress through the current day
        float normalizedTime = m_timePassed / DAY_IN_SECONDS;

        // Convert normalized time to hours and minutes
        int totalSeconds = Mathf.FloorToInt(normalizedTime * 24 * 60 * 60);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;

        // Determine AM/PM
        string period = hours >= 12 ? "pm" : "am";

        // Convert to 12-hour format
        int displayHours = hours % 12;
        if (displayHours == 0)
            displayHours = 12;

        // Format the time string
        return $"{displayHours}:{minutes:D2}{period}";
    }

    public string MapDayToString() => WEEK_DAYS[m_currentDay];

    public int GetCurrentDay() => m_currentDay;
    public int GetCurrentWeek() => m_currentWeek;
}
