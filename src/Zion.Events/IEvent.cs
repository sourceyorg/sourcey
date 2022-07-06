namespace Zion.Events
{
    public interface IEvent
    {
        EventId Id { get; }
        string Subject { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
    }
}
