using Sourcey.Aggregates;
using Sourcey.Projections;

namespace Sourcey.Events.Stores;

public interface IEventStoreContextFactory
{
    IEventStoreContext Create<TAggregate, TState>()
        where TState : IAggregateState, new()
        where TAggregate : Aggregate<TState>;

    IEventStoreContext Create<TProjection>()
        where TProjection : IProjection;
}