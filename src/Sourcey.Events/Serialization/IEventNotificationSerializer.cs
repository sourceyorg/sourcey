using Sourcey.Events.Bus;

namespace Sourcey.Events.Serialization
{
    public interface IEventNotificationSerializer
    {
        byte[] Serialize<T>(IEventNotification<T> data)
            where T : IEvent;
    }
}
