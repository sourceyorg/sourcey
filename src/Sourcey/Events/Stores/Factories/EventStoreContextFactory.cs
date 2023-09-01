using System.Collections.ObjectModel;
using Sourcey.Aggregates;
using Sourcey.Projections;

namespace Sourcey.Events.Stores;

internal sealed class EventStoreContextFactory : IEventStoreContextFactory
{
    private readonly ReadOnlyDictionary<Type, Func<IEventStoreContext>> _contexts;

    public EventStoreContextFactory(
        IEnumerable<AggregateEventContextCache> aggregateEventContextCaches,
        IEnumerable<ProjectionEventContextCache> projectionEventContextCaches)
    {
        var aggreagteDictionary = aggregateEventContextCaches.ToDictionary(
            aggregateEventContextCache => aggregateEventContextCache.AggregateType,
            aggregateEventContextCache => aggregateEventContextCache.EventContextFactory
        );

        var projectionDictionary = projectionEventContextCaches.ToDictionary(
            projectionEventContextCache => projectionEventContextCache.AggregateType,
            projectionEventContextCache => projectionEventContextCache.EventContextFactory
        );

        _contexts = new ReadOnlyDictionary<Type, Func<IEventStoreContext>>(
            aggreagteDictionary
                .Concat(projectionDictionary)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        );
    }

    public IEventStoreContext Create<TAggregate, TState>()
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        if (_contexts.TryGetValue(typeof(TAggregate), out var contextFactory))
        {
            return contextFactory();
        }

        throw new InvalidOperationException($"No event context factory found for aggregate {typeof(TAggregate).Name}");
    }

    public IEventStoreContext Create<TProjection>() where TProjection : IProjection
    {
        if (_contexts.TryGetValue(typeof(TProjection), out var contextFactory))
        {
            return contextFactory();
        }

        throw new InvalidOperationException($"No event context factory found for projection {typeof(TProjection).Name}");
    }
}