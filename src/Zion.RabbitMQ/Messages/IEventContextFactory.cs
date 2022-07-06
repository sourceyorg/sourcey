using Zion.Events;
using Zion.Events.Stores;

namespace Zion.RabbitMQ.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(ReceivedMessage message);
    }
}
