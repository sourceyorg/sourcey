using Sourcey.Events.Bus;

namespace Sourcey.Events.Serialization
{
    public interface IEventNotificationDeserializer
    {
        IEventNotification<IEvent> Deserialize(byte[] data, Type eventType);
    }
}
