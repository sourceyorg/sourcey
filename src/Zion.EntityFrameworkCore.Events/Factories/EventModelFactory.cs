using Zion.Core.Extensions;
using Zion.Events;
using Zion.Events.Serialization;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.EntityFrameworkCore.Events.Factories
{
    internal sealed class EventModelFactory : IEventModelFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public EventModelFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
        }

        public Entities.Event Create(StreamId streamId, IEventContext<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var type = @event.GetType();

            return new Entities.Event
            {
                StreamId = streamId,
                Correlation = context.Correlation,
                Causation = context.Causation,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FriendlyFullName(),
                Timestamp = @event.Timestamp,
                Actor = context.Actor,
                ScheduledPublication = context.ScheduledPublication
            };
        }
    }
}
