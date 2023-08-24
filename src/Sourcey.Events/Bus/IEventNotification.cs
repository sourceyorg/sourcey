using Sourcey.Core.Keys;
using Sourcey.Events.Streams;

namespace Sourcey.Events.Bus
{
    public interface IEventNotification<out TEvent> where TEvent : IEvent
    {
        StreamId StreamId { get; }
        Correlation? Correlation { get; }
        Causation? Causation { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        Actor Actor { get; }
    }
}
