using System.Text;
using Zion.Core.Extensions;
using Zion.Events;
using Zion.Events.Bus;
using Zion.Events.Serialization;

namespace Zion.RabbitMQ.Messages
{
    internal sealed class DefaultMessageFactory : IMessageFactory
    {
        private readonly IBodySerializer _bodySerializer;

        public DefaultMessageFactory(IBodySerializer bodySerializer)
        {
            if (bodySerializer == null)
                throw new ArgumentNullException(nameof(bodySerializer));
          
            _bodySerializer = bodySerializer;
        }

        public Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = typeof(TEvent).FriendlyFullName();

            return CreateMessage(eventName, (IEventNotification<IEvent>)context);
        }
        public Message CreateMessage(IEventNotification<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var eventName = context.Payload.GetType().FriendlyFullName();

            return CreateMessage(eventName, context);
        }

        private Message CreateMessage(string eventName, IEventNotification<IEvent> context)
        {
            var @event = context.Payload;
            var body = _bodySerializer.Serialize(context);

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = context.Correlation,
                CausationId = context.Causation,
                Actor = context.Actor,
                Body = body,
                StreamId = context.StreamId
            };
        }
    }
}
