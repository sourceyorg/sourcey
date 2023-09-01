using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Sourcey.Keys;
using Sourcey.Events.Cache;
using Sourcey.Events.Serialization;

namespace Sourcey.Events.Stores.InMemory;

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

    public IEventContext<IEvent> CreateContext(InMemoryEvent dbEvent)
    {
        if (dbEvent == null)
            throw new ArgumentNullException(nameof(dbEvent));

        if (!_eventTypeCache.TryGet(dbEvent.Type, out var type))
            throw new ArgumentException($"Could not find event type for '{dbEvent.Type}'");

        var @event = (IEvent)_eventDeserializer.Deserialize(dbEvent.Data, type);
        var correlationId = dbEvent.Correlation != null ? Correlation.From(dbEvent.Correlation) : (Correlation?)null;
        var causationId = dbEvent.Causation != null ? Causation.From(dbEvent.Causation) : (Causation?)null;

        if (_cache.TryGetValue(type, out var activator))
            return activator(dbEvent.StreamId, @event, correlationId, causationId, @event.Timestamp, Actor.From(dbEvent.Actor), dbEvent.ScheduledPublication);

        activator = BuildActivator(typeof(EventContext<>).MakeGenericType(type));

        _cache.TryAdd(type, activator);

        return activator(dbEvent.StreamId, @event, correlationId, causationId, @event.Timestamp, Actor.From(dbEvent.Actor), dbEvent.ScheduledPublication);
    }

    private Activator<IEventContext<IEvent>> BuildActivator(Type type)
    {
        var expectedParameterTypes = new Type[] { typeof(StreamId), type.GenericTypeArguments[0], typeof(Correlation?), typeof(Causation?), typeof(DateTimeOffset), typeof(Actor), typeof(DateTimeOffset?) };
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

    private delegate T Activator<T>(params object[] args);
}
