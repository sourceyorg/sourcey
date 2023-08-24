using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.RabbitMQ.Queues
{
    public interface IQueueMessageSender
    {
        Task SendAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
