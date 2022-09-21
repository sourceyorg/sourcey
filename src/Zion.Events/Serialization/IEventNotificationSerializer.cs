using Zion.Events.Bus;

namespace Zion.Events.Serialization
{
    public interface IEventNotificationSerializer
    {
        byte[] Serialize<T>(IEventNotification<T> data)
            where T : IEvent;
    }
}
