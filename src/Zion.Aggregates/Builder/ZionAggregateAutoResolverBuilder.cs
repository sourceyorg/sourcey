using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Aggregates.Concurrency;
using Zion.Events;

namespace Zion.Aggregates.Builder
{
    internal readonly partial struct ZionAggregateAutoResolverBuilder<TAggregateState> : IZionAggregateAutoResolverBuilder<TAggregateState>
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
            => InternalForSingle<TEvent>();

        public IZionAggregateAutoResolverBuilder<TAggregateState> For<TPrevEvent, TNextEvent>(bool includePermutation = false)
            where TPrevEvent : IEvent
            where TNextEvent : IEvent
            => InternalForMultiple<TPrevEvent, TNextEvent>();

        public IZionAggregateAutoResolverBuilder<TAggregateState> For(PermutationType permutation, params Type[] types)
        {
            var useSingle = permutation.HasFlag(PermutationType.Single);
            var useMultiple = permutation.HasFlag(PermutationType.Multiple);
            var useDuplicates = permutation.HasFlag(PermutationType.Duplicates);

            var type = GetType();
            var singleEventMethod = type.GetMethod(nameof(InternalForSingle), BindingFlags.Instance | BindingFlags.NonPublic);
            var multiEventMethod = type.GetMethod(nameof(InternalForMultiple), BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var eventType in types)
            {
                if(useSingle)
                    singleEventMethod?.MakeGenericMethod(eventType)?.Invoke(this, new object[] { });

                if(useDuplicates)
                    multiEventMethod?.MakeGenericMethod(eventType, eventType)?.Invoke(this, new object[] { false });

                if (useMultiple)
                    foreach (var innerEventType in types)
                            multiEventMethod?.MakeGenericMethod(eventType, innerEventType)?.Invoke(this, new object[] { false });
            }

            return this;
        }

        private IZionAggregateAutoResolverBuilder<TAggregateState> InternalForSingle<TEvent>() where TEvent : IEvent
        {
            _services.TryAddScoped<IConflictResolution<TAggregateState, TEvent>, AutoConflictResolution<TAggregateState, TEvent>>();
            return this;
        }

        private IZionAggregateAutoResolverBuilder<TAggregateState> InternalForMultiple<TPrevEvent, TNextEvent>(bool includePermutation = false)
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
