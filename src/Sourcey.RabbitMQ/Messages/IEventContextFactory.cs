using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.RabbitMQ.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(ReceivedMessage message);
    }
}
