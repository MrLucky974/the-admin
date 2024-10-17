public static class ExplorationEvents
{
    public static readonly EventTypeIdentifier SQUAD_STATUS_CHANGED = EventTypeIdentifier.Create("Exploration.Squad.StatusChanged");
}

public struct SquadStatusChangedEvent : IGameEvent
{
    public Squad.State PreviousState;
    public Squad.State CurrentState;
    public Squad Squad;
}