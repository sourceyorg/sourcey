using Zion.Events;
using Zion.Events.Bus;

namespace Zion.Azure.ServiceBus.Queues
{
    public interface IQueueMessageSender
    {
        Task SendAsync<TEvent>(string queue, IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(string queue, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
