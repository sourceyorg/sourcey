using Sourcey.Aggregates;
using Sourcey.Projections;
using Sourcey.Projections.Configuration;

namespace Sourcey.Events.Builder;

public interface IEventStoreBuilder
{
    IEventStoreBuilder AddAggregate<TAggregate, TAggregateState>()
        where TAggregateState : IAggregateState, new()
        where TAggregate : Aggregate<TAggregateState>;
    IEventStoreBuilder AddAggregates(params Type[] types);
    IEventStoreBuilder AddProjections(Action<IStoreProjectorOptions>? action = null);
    IEventStoreBuilder AddProjection<TProjection>()
        where TProjection : class, IProjection;
    IEventStoreBuilder AddProjection<TProjection>(Action<IStoreProjectorOptions>? action = null)
        where TProjection : class, IProjection;
    IEventStoreBuilder AddProjections(params Type[] types);
    IEventStoreBuilder AddProjections(Action<IStoreProjectorOptions>? action = null, params Type[] types);
}
