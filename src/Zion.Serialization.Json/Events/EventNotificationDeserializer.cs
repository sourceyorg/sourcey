using System.Text.Json;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;
using Zion.Events.Streams;

namespace Zion.Serialization.Json.Events
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
