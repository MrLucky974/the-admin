using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public const int DAY_IN_SECONDS = 5;//10 * 60;
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

    [SerializeField, Min(0f)] private float m_timeScale = 1f;

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

#if UNITY_EDITOR

        var commandSystem = GameManager.Instance.GetCommands();
        var commandLog = GameManager.Instance.GetCommandLog();
        commandSystem.AddCommand(new CommandDefinition<Action<int>>("skipweeks", (int weeks) =>
        {
            int skippedDays = WEEK_LENGTH_IN_DAYS * weeks;
            MadeInHeaven(skippedDays);
        }));

        commandSystem.AddCommand(new CommandDefinition<Action>("skipweek", () =>
        {
            MadeInHeaven(WEEK_LENGTH_IN_DAYS);
        }));

        commandSystem.AddCommand(new CommandDefinition<Action<int>>("skipdays", (int days) =>
        {
            MadeInHeaven(days);
        }));

        commandSystem.AddCommand(new CommandDefinition<Action>("skipday", () =>
        {
            MadeInHeaven(1);
        }));

        commandSystem.AddCommand(new CommandDefinition<Action<float>>("timescale", (float timeScale) =>
        {
            if (timeScale < 0f)
            {
                commandLog.AddLogError("error: timescale value cannot be less than zero.");
                return;
            }
            SetTimeScale(timeScale);
        }));

#endif
    }

    public void UpdateTime(float deltaTime)
    {
        m_timePassed += deltaTime * m_timeScale;
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

    /// <summary>
    /// Simulates the passage of time over a specified number of days.
    /// </summary>
    /// <param name="days">The number of days to simulate.</param>
    private void MadeInHeaven(int days)
    {
        // Loop through the number of days specified
        for (int i = 0; i < days; i++)
        {
            // Invoke the event for the end of the day, passing the current day
            OnDayEnded?.Invoke(m_currentDay);

            // Increment the current day
            m_currentDay++;

            // Check if the current day exceeds the length of the week
            if (m_currentDay >= WEEK_LENGTH_IN_DAYS)
            {
                // Invoke the event for the end of the week, passing the current week
                OnWeekEnded?.Invoke(m_currentWeek);

                // Increment the current week
                m_currentWeek++;

                // Reset the current day to 0 (start of a new week)
                m_currentDay = 0;
            }

            // Reset the time passed for the new day
            m_timePassed = 0;
        }
    }

    public void SetTimeScale(float timeScale)
    {
        m_timeScale = Mathf.Max(timeScale, 0f);
    }

    public float GetTimeScale() => m_timeScale;
}
