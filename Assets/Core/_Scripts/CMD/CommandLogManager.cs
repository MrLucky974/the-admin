using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CommandLogManager : MonoBehaviour
{
    private const string COMMAND_HISTORY_LOG_FORMAT = "<color={0}>[{1}] {2}</color>";

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

    public void AddLog(string message)
    {
        AddLog(message, GameManager.GREEN);
    }

    public void AddLog(string message, Color color)
    {
        m_commandHistory.Add(string.Format(COMMAND_HISTORY_LOG_FORMAT, "#" + ColorUtility.ToHtmlStringRGBA(color), m_timeManager.MapTimeToString(), message));
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
