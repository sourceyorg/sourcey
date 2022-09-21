using Zion.Events.Bus;

namespace Zion.Events.Serialization
{
    public interface IEventNotificationDeserializer
    {
        IEventNotification<IEvent> Deserialize(byte[] data, Type eventType);
    }
}
