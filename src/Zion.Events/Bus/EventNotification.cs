using Zion.Core.Keys;
using Zion.Events.Streams;

namespace Zion.Events.Bus
{
    public sealed record EventNotification<TEvent> : IEventNotification<TEvent>
        where TEvent : IEvent
    {
        public StreamId StreamId { get; }
        public Correlation? Correlation { get; }
        public Causation? Causation { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public Actor Actor { get; }

        public EventNotification(StreamId streamId, TEvent @event, Correlation? correlation, Causation? causation, DateTimeOffset timestamp, Actor actor)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            StreamId = streamId;
            Correlation = correlation;
            Causation = causation;
            Payload = @event;
            Timestamp = timestamp;
            Actor = actor;
        }
    }
}
