using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.Azure.ServiceBus.Queues
{
    public interface IQueueMessageSender
    {
        Task SendAsync<TEvent>(string queue, IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(string queue, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
