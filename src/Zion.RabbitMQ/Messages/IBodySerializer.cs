using Zion.Events;
using Zion.Events.Bus;

namespace Zion.RabbitMQ.Messages
{
    public interface IBodySerializer
    {
        byte[] Serialize<T>(IEventNotification<T> data)
            where T : IEvent;
    }
}
