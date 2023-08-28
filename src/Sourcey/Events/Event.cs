using Sourcey.Keys;

namespace Sourcey.Events;

public abstract record Event : IEvent
{
    public EventId Id { get; protected init; }
    public StreamId StreamId { get; protected init; }
    public DateTimeOffset Timestamp { get; protected init; }
    public int? Version { get; protected init; }

    public Event()
    {

    }
    
    public Event(StreamId streamId, int? version)
    {
        Id = EventId.New();
        StreamId = streamId;
        Timestamp = DateTimeOffset.UtcNow;
        Version = version;
    }

    public Event(StreamId streamId) : this(streamId, null) { }
}
