using Sourcey.Events;
using Sourcey.Events.Bus;

namespace Sourcey.Azure.ServiceBus.Queues
{
    internal class EventQueuePublisher : IEventQueuePublisher
    {
        private readonly IQueueMessageSender _messageSender;

        public EventQueuePublisher(IQueueMessageSender messageSender)
        {
            if (messageSender is null)
                throw new ArgumentNullException(nameof(messageSender));

            _messageSender = messageSender;
        }

        public async Task PublishAsync<TEvent>(string queue, IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            await _messageSender.SendAsync(queue, context, cancellationToken);
        }

        public async Task PublishAsync(string queue, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            await _messageSender.SendAsync(queue, contexts, cancellationToken);
        }
    }
}
