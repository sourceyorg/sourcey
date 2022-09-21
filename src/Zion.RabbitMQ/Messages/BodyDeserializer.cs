using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Utf8Json;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;
using Zion.Events.Streams;

namespace Zion.RabbitMQ.Messages
{
    internal sealed class BodyDeserializer : IBodyDeserializer
    {
        private readonly IEventDeserializer _eventDeserializer;

        public BodyDeserializer(IEventDeserializer eventDeserializer)
        {
            _eventDeserializer = eventDeserializer;
        }

        public IEventNotification<IEvent> Deserialize(byte[] data, Type eventType)
        {
            var body = JsonSerializer.Deserialize<RabbitMqBody>(data);
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
