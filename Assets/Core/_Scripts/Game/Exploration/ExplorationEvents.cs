public static class ExplorationEvents
{
    public static readonly EventTypeIdentifier SQUAD_BACK_TO_BASE = EventTypeIdentifier.Create("Exploration.Squad.ArrivedToBase");
}

public struct SquadArrivalEvent : IGameEvent
{
    public (ResourceType type, int amount) resources;
}