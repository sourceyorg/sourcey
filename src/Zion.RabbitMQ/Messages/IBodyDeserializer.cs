using Zion.Events;
using Zion.Events.Bus;

namespace Zion.RabbitMQ.Messages
{
    public interface IBodyDeserializer
    {
        IEventNotification<IEvent> Deserialize(byte[] data, Type eventType);
    }
}
