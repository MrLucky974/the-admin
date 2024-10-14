using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CommandLogManager : MonoBehaviour
{
    private const string COMMAND_HISTORY_LOG_FORMAT = "[{0}] {1}";

    private List<string> m_commandHistory = new List<string>();
    private Action m_historyChanged;

    private TimeManager m_timeManager;

    private void Start()
    {
        m_timeManager = GameManager.Instance.GetTimeManager();
    }

    private void OnDestroy()
    {
        m_historyChanged = null;
    }

    public ReadOnlyCollection<string> GetCommandHistory()
    {
        return m_commandHistory.AsReadOnly();
    }

    public void AddLog(string message, bool format = true)
    {
        AddLog(message, GameManager.GREEN, format);
    }

    public void AddLogError(string message, bool format = true)
    {
        AddLog(message, GameManager.RED, format);
        SoundManager.PlaySound(SoundType.ERROR);
    }

    public void AddLog(string message, Color color, bool format = true)
    {
        string text = "";
        if (format is true)
        {
            text = string.Format(COMMAND_HISTORY_LOG_FORMAT, m_timeManager.MapTimeToString(), message);
        }
        else
        {
            text = message;
        }

        m_commandHistory.Add(JUtils.FormatColor(text, color));
        m_historyChanged?.Invoke();
    }

    public void Clear()
    {
        m_commandHistory.Clear();
        m_historyChanged?.Invoke();
    }

    public void RegisterOnHistoryChanged(Action action)
    {
        m_historyChanged += action;
    }

    public void UnregisterOnHistoryChanged(Action action)
    {
        m_historyChanged -= action;
    }
}
