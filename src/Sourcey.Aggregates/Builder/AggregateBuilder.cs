using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Events;

namespace Sourcey.Aggregates.Builder
{
    public class AggregateBuilder<TAggregate, TAggregateState> : IAggregateBuilder<TAggregate, TAggregateState>
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        public IServiceCollection Services { get; }

        public AggregateBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;

            Services.TryAddScoped<IAggregateFactory, AggregateFactory>();
            Services.TryAddScoped<IConflictResolver, ConflictResolver>();
        }

        public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>() 
            where TConflictResolution : class, IConflictResolution<TAggregateState>
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState>, TConflictResolution>();
            return this;
        }

        public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
            where TEvent : IEvent
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState, TEvent>, TConflictResolution>();
            return this;
        }

        public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
            where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
            where TPrevEvent : IEvent
            where TNextEvent : IEvent
        {
            Services.TryAddScoped<IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>, TConflictResolution>();
            return this;
        }

        public IAggregateBuilder<TAggregate, TAggregateState> WithAutoResolution(Action<IAggregateAutoResolverBuilder<TAggregateState>> configuration)
        {
            var sourceyAggregateAutoResolverBuilder = new AggregateAutoResolverBuilder<TAggregateState>(Services);
            configuration(sourceyAggregateAutoResolverBuilder);
            return this;
        }

        public IAggregateBuilder<TAggregate, TAggregateState> WithSnapshotStrategy<TSnapshot, TSnapshooter>(SnapshotExecution execution)
            where TSnapshot : class, IAggregateSnapshot<TAggregate, TAggregateState>
            where TSnapshooter : class, IAggregateSnapshooter<TAggregateState>
        {
            Services.AddSingleton<IAggregateSnapshot<TAggregate, TAggregateState>, TSnapshot>();

            if (execution == SnapshotExecution.Sync)
            {
                Services.AddSingleton<IAggregateSnapshooter<TAggregateState>, TSnapshooter>();
                return this;
            }

            Services.AddSingleton<TSnapshooter>();
            Services.AddSingleton(sp => new BufferedAggregateSnapshooter<TAggregateState>(sp.GetRequiredService<TSnapshooter>()));
            Services.AddSingleton<IAggregateSnapshooter<TAggregateState>>(sp => sp.GetRequiredService<BufferedAggregateSnapshooter<TAggregateState>>());
            Services.AddHostedService(sp => sp.GetRequiredService<BufferedAggregateSnapshooter<TAggregateState>>());

            return this;
        }
    }
}
