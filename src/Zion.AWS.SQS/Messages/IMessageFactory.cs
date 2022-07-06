using Amazon.SQS.Model;
using Zion.Events;
using Zion.Events.Bus;

namespace Zion.AWS.SQS.Messages
{
    internal interface IMessageFactory
    {
        IEnumerable<SendMessageRequest> CreateMessages<TEvent>(IEnumerable<SQSQueue> queues, IEventNotification<TEvent> context, CancellationToken cancellationToken = default)
            where TEvent: IEvent;

        IEnumerable<SendMessageBatchRequest> CreateBatchMessages(IEnumerable<SQSQueue> queues, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
