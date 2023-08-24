using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.Azure.ServiceBus.Topics
{
    public interface ITopicMessageSender
    {
        Task SendAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
