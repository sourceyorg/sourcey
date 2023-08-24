using Sourcey.Events;

namespace Sourcey.Aggregates.Builder
{
    public interface IAggregateAutoResolverBuilder<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        IAggregateAutoResolverBuilder<TAggregateState> For<TEvent>()
            where TEvent : IEvent;
        IAggregateAutoResolverBuilder<TAggregateState> For<TPrevEvent, TNextEvent>(bool includePermutation = false)
            where TPrevEvent : IEvent
            where TNextEvent : IEvent;

        IAggregateAutoResolverBuilder<TAggregateState> For(PermutationType permutation, params Type[] events);
    }
}
