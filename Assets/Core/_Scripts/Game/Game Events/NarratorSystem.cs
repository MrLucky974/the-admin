using System;
using System.Collections.Generic;
using UnityEngine;

public class NarratorSystem : MonoBehaviour
{
    private Dictionary<EventTypeIdentifier, Delegate> m_events = new();

    public void Initialize()
    {

    }

    /// <summary>
    /// Connect a delegate to the event specified by an identifier.
    /// </summary>
    /// <typeparam name="T">Data structure of the event.</typeparam>
    /// <param name="eventType">Identifier of the event.</param>
    /// <param name="listener">Delegate listening to the event.</param>
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

    /// <summary>
    /// Remove the delegate from the event specified by an identifier.
    /// </summary>
    /// <typeparam name="T">Data structure of the event.</typeparam>
    /// <param name="eventType">Identifier of the event.</param>
    /// <param name="listener">Delegate listening to the event.</param>
    public void Unsubscribe<T>(EventTypeIdentifier eventType, Action<T> listener) where T : IGameEvent
    {
        if (m_events.ContainsKey(eventType))
        {
            m_events[eventType] = Delegate.Remove(m_events[eventType], listener);
        }
    }

    /// <summary>
    /// Send a message to every listener of the event specified by an identifier.
    /// </summary>
    /// <typeparam name="T">Data structure of the event.</typeparam>
    /// <param name="eventType">Identifier of the event.</param>
    /// <param name="eventData">Data values of the event.</param>
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