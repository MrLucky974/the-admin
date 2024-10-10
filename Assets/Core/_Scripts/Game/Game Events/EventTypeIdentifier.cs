public class EventTypeIdentifier
{
    private readonly string id;

    private EventTypeIdentifier(string id)
    {
        this.id = id;
    }

    public override bool Equals(object obj)
    {
        return obj is EventTypeIdentifier other && id == other.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public static EventTypeIdentifier Create(string id)
    {
        return new EventTypeIdentifier(id);
    }
}
