
public static class RoomEvents
{ 
    // Start is called before the first frame update
    public static readonly EventTypeIdentifier DAMAGED_ROOM = EventTypeIdentifier.Create("Base.Room.DamagedByFlooding");

}

public struct DamagedRoomEvent : IGameEvent
{
    public RoomType roomType;
    public int damage;
}
