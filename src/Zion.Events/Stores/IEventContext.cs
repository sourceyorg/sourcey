using Zion.Core.Keys;
using Zion.Events.Streams;

namespace Zion.Events.Stores
{
    public interface IEventContext<out TEvent>
        where TEvent : IEvent
    {
        StreamId StreamId { get; }
        Correlation? Correlation { get; }
        Causation? Causation { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        Actor Actor { get; }
        DateTimeOffset? ScheduledPublication { get; }
    }
}
