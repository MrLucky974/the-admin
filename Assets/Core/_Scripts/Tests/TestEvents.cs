#if UNITY_EDITOR

using UnityEngine;

public static class TestEvents
{
    public static readonly EventTypeIdentifier Test_01 = EventTypeIdentifier.Create("Test.Test01");
}

public struct TestEventData : IGameEvent
{
    public string stringValue;
    public int intValue;
    public Vector3 vecValue;
}

public struct FooEventData : IGameEvent
{
    public string stringValue;
}

#endif