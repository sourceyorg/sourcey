using Amazon.SQS.Model;
using Zion.Core.Extensions;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;

namespace Zion.AWS.SQS.Messages
{
    internal sealed class MessageFactory : IMessageFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public MessageFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer is null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
        }

        public IEnumerable<SendMessageBatchRequest> CreateBatchMessages(IEnumerable<SQSQueue> queues, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            var entries = CreateBatchEntries(queues, contexts, cancellationToken).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var queue in queues)
                yield return new SendMessageBatchRequest(queue, entries[queue].ToList());
        }

        public IEnumerable<SendMessageRequest> CreateMessages<TEvent>(IEnumerable<SQSQueue> queues, IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            var data = CreateData((IEventNotification<IEvent>)context);

            foreach(var queue in queues)
                yield return new(queue, data.body) { MessageAttributes = data.attributes, MessageGroupId = queue.ToString() };
        }

        private IEnumerable<KeyValuePair<SQSQueue, IEnumerable<SendMessageBatchRequestEntry>>> CreateBatchEntries(IEnumerable<SQSQueue> queues, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default)
        {
            foreach(var queue in queues)
            {
                var entries = new List<SendMessageBatchRequestEntry>();

                foreach (var context in contexts)
                {
                    var data = CreateData(context);
                    entries.Add(new SendMessageBatchRequestEntry(context.Payload.Id, data.body)
                    {
                        MessageAttributes = data.attributes,
                        MessageGroupId = queue.ToString()
                    });
                }

                yield return new(queue, entries);
            }
            
        }

        private (string body, Dictionary<string, MessageAttributeValue> attributes) CreateData(IEventNotification<IEvent> context)
            => (
                _eventSerializer.Serialize(context.Payload),
                new()
                {
                    [MessageConstants.LabelKey] = CreateValue(context.Payload.GetType().FriendlyFullName()),
                    [MessageConstants.StreamIdKey] = CreateValue(context.StreamId.ToString()),
                    [MessageConstants.CausationKey] = CreateValue(context.Causation?.ToString()),
                    [MessageConstants.ActorKey] = CreateValue(context.Actor.ToString()),
                    [MessageConstants.CorrealtionKey] = CreateValue(context.Correlation.ToString()),
                    [MessageConstants.TimestampKey] = CreateValue(context.Timestamp.ToString())
                }
            );

        private MessageAttributeValue CreateValue(string? value) => new() { DataType = "String", StringValue = value ?? string.Empty };
    }
}
