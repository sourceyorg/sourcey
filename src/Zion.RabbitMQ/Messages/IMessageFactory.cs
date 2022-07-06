using Zion.Events;
using Zion.Events.Bus;

namespace Zion.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventNotification<IEvent> context);
    }
}
