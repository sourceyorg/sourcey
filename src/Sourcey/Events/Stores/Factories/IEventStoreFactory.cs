using Sourcey.Aggregates;
using Sourcey.Projections;

namespace Sourcey.Events.Stores;

public interface IEventStoreFactory
{
    IEventStore Create<TAggregate, TState>()
        where TState : IAggregateState, new()
        where TAggregate : Aggregate<TState>;

    IEventStore Create<TProjection>()
        where TProjection : IProjection;
}