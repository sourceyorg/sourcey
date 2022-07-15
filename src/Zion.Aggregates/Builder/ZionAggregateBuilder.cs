﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Aggregates.Concurrency;

using Zion.Events;

namespace Zion.Aggregates.Builder
{
    public class ZionAggregateBuilder<TAggregate, TAggregateState> : IZionAggregateBuilder<TAggregate, TAggregateState>
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        public IServiceCollection Services { get; }

        public ZionAggregateBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;

            Services.TryAddScoped<IAggregateFactory, AggregateFactory>();
            Services.TryAddScoped<IConflictResolver, ConflictResolver>();
        }

        public IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>() 
            where TConflictResolution : class, IConflictResolution<TAggregateState>
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState>, TConflictResolution>();
            return this;
        }

        public IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
            where TEvent : IEvent
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState, TEvent>, TConflictResolution>();
            return this;
        }

        public IZionAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
            where TPrevEvent : IEvent
            where TNextEvent : IEvent
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>, TConflictResolution>();
            return this;
        }

        public IZionAggregateBuilder<TAggregate, TAggregateState> WithAutoResolution(Action<IZionAggregateAutoResolverBuilder<TAggregateState>> configuration)
        {
            var zionAggregateAutoResolverBuilder = new ZionAggregateAutoResolverBuilder<TAggregateState>(Services);
            configuration(zionAggregateAutoResolverBuilder);
            return this;
        }
    }
}
