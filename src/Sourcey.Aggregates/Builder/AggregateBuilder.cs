using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Events;

namespace Sourcey.Aggregates.Builder;

internal readonly struct AggregateBuilder<TAggregate, TAggregateState> : IAggregateBuilder<TAggregate, TAggregateState>
    where TAggregate : Aggregate<TAggregateState>
    where TAggregateState : IAggregateState, new()
{
    private readonly IServiceCollection _services;

    public AggregateBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;

        _services.TryAddScoped<IAggregateFactory, AggregateFactory>();
        _services.TryAddScoped<IConflictResolver, ConflictResolver>();
    }

    public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution>() 
        where TConflictResolution : class, IConflictResolution<TAggregateState>
    {
        _services.TryAddScoped<IConflictResolution<TAggregateState>, TConflictResolution>();
        return this;
    }

    public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TEvent>
        where TEvent : IEvent
    {
        _services.TryAddScoped<IConflictResolution<TAggregateState, TEvent>, TConflictResolution>();
        return this;
    }

    public IAggregateBuilder<TAggregate, TAggregateState> WithConflictResolution<TConflictResolution, TPrevEvent, TNextEvent>()
        where TConflictResolution : class, IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>
        where TPrevEvent : IEvent
        where TNextEvent : IEvent
    {
        _services.TryAddScoped<IConflictResolution<TAggregateState, TPrevEvent, TNextEvent>, TConflictResolution>();
        return this;
    }

    public IAggregateBuilder<TAggregate, TAggregateState> WithAutoResolution(Action<IAggregateAutoResolverBuilder<TAggregateState>> configuration)
    {
        var sourceyAggregateAutoResolverBuilder = new AggregateAutoResolverBuilder<TAggregateState>(_services);
        configuration(sourceyAggregateAutoResolverBuilder);
        return this;
    }

    public IAggregateBuilder<TAggregate, TAggregateState> WithSnapshotStrategy<TSnapshot, TSnapshooter>(SnapshotExecution execution)
        where TSnapshot : class, IAggregateSnapshot<TAggregate, TAggregateState>
        where TSnapshooter : class, IAggregateSnapshooter<TAggregateState>
    {
        _services.AddSingleton<IAggregateSnapshot<TAggregate, TAggregateState>, TSnapshot>();

        if (execution == SnapshotExecution.Sync)
        {
            _services.AddSingleton<IAggregateSnapshooter<TAggregateState>, TSnapshooter>();
            return this;
        }

        _services.AddSingleton<TSnapshooter>();
        _services.AddSingleton(sp => new BufferedAggregateSnapshooter<TAggregateState>(sp.GetRequiredService<TSnapshooter>()));
        _services.AddSingleton<IAggregateSnapshooter<TAggregateState>>(sp => sp.GetRequiredService<BufferedAggregateSnapshooter<TAggregateState>>());
        _services.AddHostedService(sp => sp.GetRequiredService<BufferedAggregateSnapshooter<TAggregateState>>());

        return this;
    }
}
