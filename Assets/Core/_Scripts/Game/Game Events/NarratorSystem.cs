using System;
using System.Collections.Generic;
using UnityEngine;

// TODO : Recieve special event requests from other scripts
// TODO : Send event data to listeners
public class NarratorSystem : MonoBehaviour
{
    private Dictionary<EventTypeIdentifier, Delegate> m_events = new();

    public void Initialize()
    {

    }

    public void Subscribe<T>(EventTypeIdentifier eventType, Action<T> listener) where T : IGameEvent
    {
        if (!m_events.ContainsKey(eventType))
        {
            m_events[eventType] = listener;
        }
        else
        {
            m_events[eventType] = Delegate.Combine(m_events[eventType], listener);
        }
    }

    public void Unsubscribe<T>(EventTypeIdentifier eventType, Action<T> listener) where T : IGameEvent
    {
        if (m_events.ContainsKey(eventType))
        {
            m_events[eventType] = Delegate.Remove(m_events[eventType], listener);
        }
    }

    public void TriggerEvent<T>(EventTypeIdentifier eventType, T eventData) where T : IGameEvent
    {
        if (m_events.TryGetValue(eventType, out Delegate del))
        {
            if (del is Action<T> callback)
            {
                callback.Invoke(eventData);
            }
        }
    }
}