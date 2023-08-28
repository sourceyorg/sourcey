using Sourcey.Keys;

namespace Sourcey.Events;

public interface IEvent
{
    EventId Id { get; }
    StreamId StreamId { get; }
    DateTimeOffset Timestamp { get; }
    int? Version { get; }
}
