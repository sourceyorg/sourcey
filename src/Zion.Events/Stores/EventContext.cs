using Zion.Core.Keys;
using Zion.Events.Streams;

namespace Zion.Events.Stores
{
    public sealed record EventContext<TEvent> : IEventContext<TEvent>
        where TEvent : IEvent
    {
        public StreamId StreamId { get; }
        public Correlation? Correlation { get; }
        public Causation? Causation { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public Actor Actor { get; }
        public DateTimeOffset? ScheduledPublication { get; }

        public EventContext(StreamId streamId, TEvent @event, Correlation? correlation, Causation? causation, DateTimeOffset timestamp, Actor actor, DateTimeOffset? scheduledPublication = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            StreamId = streamId;
            Correlation = correlation;
            Causation = causation;
            Payload = @event;
            Timestamp = timestamp;
            Actor = actor;
            ScheduledPublication = scheduledPublication;
        }
    }
}
