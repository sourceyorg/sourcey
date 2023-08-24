using System.Text.Json;
using Sourcey.Core.Keys;
using Sourcey.Events;
using Sourcey.Events.Bus;
using Sourcey.Events.Serialization;
using Sourcey.Events.Streams;

namespace Sourcey.Serialization.Json.Events
{
    internal sealed class EventNotificationDeserializer : IEventNotificationDeserializer
    {
        private readonly IEventDeserializer _eventDeserializer;

        public EventNotificationDeserializer(IEventDeserializer eventDeserializer)
        {
            _eventDeserializer = eventDeserializer;
        }

        public IEventNotification<IEvent> Deserialize(byte[] data, Type eventType)
        {
            var body = JsonSerializer.Deserialize<EventNotificationPayload>(data);
            return new EventNotification<IEvent>(
                streamId: StreamId.From(body.StreamId),
                @event: (IEvent)_eventDeserializer.Deserialize(body.Payload, eventType),
                correlation: string.IsNullOrWhiteSpace(body.Correlation) ? null : Correlation.From(body.Correlation),
                causation: string.IsNullOrWhiteSpace(body.Causation) ? null : Causation.From(body.Causation),
                timestamp: body.Timestamp,
                actor: Actor.From(body.Actor));
        }
    }
}
