using Utf8Json;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;

namespace Zion.RabbitMQ.Messages
{
    internal class BodySerializer : IBodySerializer
    {
        private readonly IEventSerializer _eventSerializer;

        public BodySerializer(IEventSerializer eventSerializer)
        {
            _eventSerializer = eventSerializer;
        }

        public byte[] Serialize<T>(IEventNotification<T> data) where T : IEvent
        {
            var body = new RabbitMqBody
            {
                Actor = data.Actor,
                Causation = data.Causation,
                Correlation = data.Correlation,
                Payload = _eventSerializer.Serialize(data.Payload),
                StreamId = data.StreamId,
                Timestamp = data.Timestamp
            };

            return JsonSerializer.Serialize(body);
        }
    }
}
