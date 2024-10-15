public static class ExplorationEvents
{
    public static readonly EventTypeIdentifier SQUAD_BACK_TO_BASE = EventTypeIdentifier.Create("Exploration.Squad.ArrivedToBase");
    public static readonly EventTypeIdentifier SQUAD_STATUS_CHANGED = EventTypeIdentifier.Create("Exploration.Squad.StatusChanged");
}

public struct SquadArrivalEvent : IGameEvent
{
    public (ResourceType type, int amount) Resources;
    public VillagerData[] Members;
}

public struct SquadStatusChangedEvent : IGameEvent
{
    public Squad.State PreviousState;
    public Squad.State CurrentState;
    public Squad Squad;
}