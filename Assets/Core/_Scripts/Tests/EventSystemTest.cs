using UnityEngine;

public class EventSystemTest : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        var narrator = GameManager.Instance.GetNarrator();
        narrator.Subscribe<TestEventData>(TestEvents.Test_01, OnTestEvent);

        // IT WORKS!!
        var data = new TestEventData
        {
            stringValue = "test",
            intValue = 10,
            vecValue = Vector3.one,
        };
        narrator.TriggerEvent<TestEventData>(TestEvents.Test_01, data);

        // It doesn't work... WHICH IS EXACTLY WHAT IS SUPPOSED TO HAPPEN!!!
        var foo = new FooEventData
        {
            stringValue = "test",
        };
        narrator.TriggerEvent<FooEventData>(TestEvents.Test_01, foo);
    }

    void OnTestEvent(TestEventData data)
    {
        Debug.Log(data.stringValue);
        Debug.Log(data.intValue);
        Debug.Log(data.vecValue);
    }
#endif
}
