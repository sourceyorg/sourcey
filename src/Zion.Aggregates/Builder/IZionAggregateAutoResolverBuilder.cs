using Zion.Events;

namespace Zion.Aggregates.Builder
{
    public interface IZionAggregateAutoResolverBuilder<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        IZionAggregateAutoResolverBuilder<TAggregateState> For<TEvent>()
            where TEvent : IEvent;
        IZionAggregateAutoResolverBuilder<TAggregateState> For<TPrevEvent, TNextEvent>(bool includePermutation = false)
            where TPrevEvent : IEvent
            where TNextEvent : IEvent;
    }
}
