using Sourcey.Aggregates;
using Sourcey.Projections;

namespace Sourcey.Events.Stores;

internal sealed class EventStoreFactory : IEventStoreFactory
{
    private readonly IEventStoreContextFactory _contextFactory;
    private readonly IServiceProvider _serviceProvider;

    public EventStoreFactory(IEventStoreContextFactory contextFactory, IServiceProvider serviceProvider)
    {
        _contextFactory = contextFactory;
        _serviceProvider = serviceProvider;
    }

    public IEventStore Create<TAggregate, TState>()
        where TAggregate : Aggregate<TState>
        where TState : IAggregateState, new()
    {
        var context = _contextFactory.Create<TAggregate, TState>();
        var eventStoreType = typeof(IEventStore<>).MakeGenericType(context.GetType());

        var eventStore = _serviceProvider.GetService(eventStoreType);

        if (eventStore is null)
            throw new InvalidOperationException($"No event store found for context {context.GetType().Name}");

        return (IEventStore)eventStore;
    }

    public IEventStore Create<TProjection>() where TProjection : IProjection
    {
        var context = _contextFactory.Create<TProjection>();
        var eventStoreType = typeof(IEventStore<>).MakeGenericType(context.GetType());

        var eventStore = _serviceProvider.GetService(eventStoreType);

        if (eventStore is null)
            throw new InvalidOperationException($"No event store found for context {context.GetType().Name}");

        return (IEventStore)eventStore;
    }
}