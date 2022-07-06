using Microsoft.Extensions.DependencyInjection;
using Zion.Aggregates.Concurrency;
using Zion.Events;

namespace Zion.Aggregates.Builder
{
    public interface IZionAggregateBuilder<TAggregate, TAggregateState>
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        IServiceCollection Services { get; }

        IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>()
            where TConflictResolution : class, IConflictResolution<TAggregateState>;

        IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
            where TEvent : IEvent;

        IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
            where TPrevEvent : IEvent
            where TNextEvent : IEvent;
    }
}
