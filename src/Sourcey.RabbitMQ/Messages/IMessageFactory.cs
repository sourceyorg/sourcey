using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventNotification<IEvent> context);
    }
}
