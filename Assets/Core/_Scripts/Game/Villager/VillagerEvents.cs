

public class VillagerEvents 
{
    public static readonly EventTypeIdentifier VILLAGER_AT_DOOR = EventTypeIdentifier.Create("Villager.AtDoor");
}


public struct VillagerAtDoorEvent : IGameEvent
{
    public VillagerData newVillager;
}