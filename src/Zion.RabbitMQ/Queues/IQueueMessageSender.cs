using Zion.Events;
using Zion.Events.Bus;

namespace Zion.RabbitMQ.Queues
{
    public interface IQueueMessageSender
    {
        Task SendAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
