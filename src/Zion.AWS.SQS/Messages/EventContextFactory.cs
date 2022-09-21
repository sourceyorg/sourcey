using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Amazon.SQS.Model;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Cache;
using Zion.Events.Serialization;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.AWS.SQS.Messages
{
    internal sealed class EventContextFactory : IEventContextFactory
    {
        private readonly ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>> _cache;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly IEventDeserializer _eventDeserializer;

        public EventContextFactory(IEventTypeCache eventTypeCache,
                                          IEventDeserializer eventDeserializer)
        {
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));

            _eventTypeCache = eventTypeCache;
            _eventDeserializer = eventDeserializer;
            _cache = new ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>>();
        }

        public IEventContext<IEvent> CreateContext(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!message.Attributes.TryGetValue(MessageConstants.LabelKey, out var label) || !string.IsNullOrWhiteSpace(label))
                throw new ArgumentException($"Could not find event label for '{message.MessageId}'");

            if (!_eventTypeCache.TryGet(label, out var type) || type is null)
                throw new ArgumentException($"Could not find event type for '{label}'");

            var @event = (IEvent)_eventDeserializer.Deserialize(message.Body, type);

            StreamId? streamId = null;
            Correlation? correlationId = null;
            Causation? causationId = null;
            Actor? actor = null;

            if (message.Attributes.TryGetValue(MessageConstants.StreamIdKey, out var streamIdString) && !string.IsNullOrWhiteSpace(streamIdString))
                streamId = StreamId.From(streamIdString);
            if (message.Attributes.TryGetValue(MessageConstants.CorrealtionKey, out var correlationString) && !string.IsNullOrWhiteSpace(correlationString))
                correlationId = Correlation.From(correlationString);
            if (message.Attributes.TryGetValue(MessageConstants.CausationKey, out var causationString) && !string.IsNullOrWhiteSpace(causationString))
                causationId = Causation.From(causationString);
            if (message.Attributes.TryGetValue(MessageConstants.ActorKey, out var actorString) && !string.IsNullOrWhiteSpace(actorString))
                actor = Actor.From(actorString);

            if (_cache.TryGetValue(type, out var activator))
                return activator(streamId, @event, correlationId, causationId, @event.Timestamp, Actor.From(actor ?? "<Unknown>"));

            activator = BuildActivator(typeof(EventContext<>).MakeGenericType(type));

            _cache.TryAdd(type, activator);

            return activator(streamId, @event, correlationId, causationId, @event.Timestamp, Actor.From(actor ?? "<Unknown>"));
        }

        private Activator<IEventContext<IEvent>> BuildActivator(Type type)
        {
            var expectedParameterTypes = new Type[] { typeof(StreamId), type.GenericTypeArguments[0], typeof(Correlation?), typeof(Causation?), typeof(DateTimeOffset), typeof(Actor) };
            var constructor = type.GetConstructor(expectedParameterTypes);

            if (constructor == null)
                throw new InvalidOperationException($"Could not find expected constructor on type '{type.FullName}'. Expecting the following parameters: '{string.Join("' ,", expectedParameterTypes.Select(t => t.Name))}'");

            var parameters = constructor.GetParameters();
            var args = Expression.Parameter(typeof(object[]), "args");
            var argsExpressions = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var accessor = Expression.ArrayIndex(args, Expression.Constant(i));
                var convert = Convert(accessor, parameterType);
                argsExpressions[i] = convert;
            }

            var newExpression = Expression.New(constructor, argsExpressions);
            var lambda = Expression.Lambda(typeof(Activator<IEventContext<IEvent>>), newExpression, args);

            return (Activator<IEventContext<IEvent>>)lambda.Compile();
        }

        private static Expression Convert(Expression expression, Type type)
        {
            if (type.GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
                return expression;

            return Expression.Convert(expression, type);
        }

        private delegate T Activator<T>(params object?[] args);
    }
}
