using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Aggregates.Concurrency;
using Zion.Events;

namespace Zion.Aggregates.Builder
{
    internal readonly struct ZionAggregateAutoResolverBuilder<TAggregateState> : IZionAggregateAutoResolverBuilder<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        private readonly IServiceCollection _services;

        public ZionAggregateAutoResolverBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IZionAggregateAutoResolverBuilder<TAggregateState> For<TEvent>() where TEvent : IEvent
        {
            _services.TryAddScoped<IConflictResolution<TAggregateState, TEvent>, AutoConflictResolution<TAggregateState, TEvent>>();
            return this;
        }

        public IZionAggregateAutoResolverBuilder<TAggregateState> For<TPrevEvent, TNextEvent>(bool includePermutation = false)
            where TPrevEvent : IEvent
            where TNextEvent : IEvent
        {
            _services.TryAddScoped<IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>, AutoConflictResolution<TAggregateState, TPrevEvent, TNextEvent>>();
            
            if (includePermutation)
                _services.TryAddScoped<IConflictResolution<TAggregateState, TNextEvent, TPrevEvent>, AutoConflictResolution<TAggregateState, TNextEvent, TPrevEvent>>();
            
            return this;
        }
    }
}
