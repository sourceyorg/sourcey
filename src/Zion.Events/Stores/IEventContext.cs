using Zion.Core.Keys;
namespace Zion.Events.Stores
{
    public interface IEventContext<out TEvent>
        where TEvent : IEvent
    {
        string StreamId { get; }
        Correlation? Correlation { get; }
        Causation? Causation { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        Actor Actor { get; }
        DateTimeOffset? ScheduledPublication { get; }
    }
}
