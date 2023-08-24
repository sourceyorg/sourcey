using Sourcey.Events.Streams;

namespace Sourcey.Events
{
    public interface IEvent
    {
        EventId Id { get; }
        StreamId StreamId { get; }
        DateTimeOffset Timestamp { get; }
        int? Version { get; }
    }
}
