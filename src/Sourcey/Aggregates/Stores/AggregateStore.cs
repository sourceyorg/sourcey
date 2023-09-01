using Microsoft.Extensions.DependencyInjection;
using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Exceptions;
using Sourcey.Keys;
using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.Aggregates.Stores;

/// <summary>
/// <inheritdoc/>
/// </summary>
internal sealed class AggregateStore<TAggregate, TState> : IAggregateStore<TAggregate, TState>
    where TState : IAggregateState, new()
    where TAggregate : Aggregate<TState>
{
    private readonly IEventStoreFactory _eventStoreFactory;
    private readonly IAggregateFactory _aggregateFactory;
    private readonly IConflictResolver _conflictResolver;
    private readonly IExceptionStream _exceptionStream;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AggregateStore(IEventStoreFactory eventStoreFactory,
        IAggregateFactory aggregateFactory,
        IConflictResolver conflictResolver,
        IExceptionStream exceptionStream,
        IServiceScopeFactory serviceScopeFactory)
    {
        if (eventStoreFactory == null)
            throw new ArgumentNullException(nameof(eventStoreFactory));
        if (aggregateFactory == null)
            throw new ArgumentNullException(nameof(aggregateFactory));
        if (conflictResolver == null)
            throw new ArgumentNullException(nameof(conflictResolver));
        if (exceptionStream == null)
            throw new ArgumentNullException(nameof(exceptionStream));
        if (serviceScopeFactory == null)
            throw new ArgumentNullException(nameof(serviceScopeFactory));

        _eventStoreFactory = eventStoreFactory;
        _aggregateFactory = aggregateFactory;
        _conflictResolver = conflictResolver;
        _exceptionStream = exceptionStream;
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task<TAggregate?> GetAsync(StreamId id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var snapshot = await GetSnapshotAsync(id, cancellationToken);

        if (snapshot is not null)
            return snapshot;

        var events = await _eventStoreFactory.Create<TAggregate, TState>().GetEventsAsync(id, null, cancellationToken);

        if (!events.Any())
            return null;

        var aggregate = _aggregateFactory.FromHistory<TAggregate, TState>(events.Select(e => e.Payload));

        return aggregate;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
     public async Task SaveAsync(
        TAggregate aggregate,
        Actor? actor = null,
        Causation? causation = null,
        Correlation? correlation = null,
        DateTimeOffset? scheduledPublication = null,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default)
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        cancellationToken.ThrowIfCancellationRequested();

        var events = aggregate.GetUncommittedEvents();

        if (!events.Any())
            return;

        var eventStore = _eventStoreFactory.Create<TAggregate, TState>();

        var currentVersion = await eventStore.CountAsync(aggregate.Id);

        if (expectedVersion.HasValue && expectedVersion.Value != currentVersion
            && !await ResolveConflictAsync(aggregate, expectedVersion.Value, currentVersion, cancellationToken))
            return;

        events = aggregate.GetUncommittedEvents();

        var contexts = events.Select(@event => new EventContext<IEvent>(
            streamId: aggregate.Id,
            @event: @event,
            correlation: correlation,
            causation: causation,
            timestamp: @event.Timestamp,
            actor: actor ?? Actor.From("unknown"),
            scheduledPublication: scheduledPublication));

        await eventStore.SaveAsync(aggregate.Id, contexts);

        aggregate.ClearUncommittedEvents();

        await SaveSnapshotAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task SaveAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
        => await SaveAsync(
            aggregate: aggregate,
            actor: null,
            causation: null,
            correlation: null,
            scheduledPublication: null,
            expectedVersion: null,
            cancellationToken: cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task SaveAsync(
        TAggregate aggregate,
        IEventContext<IEvent> causation,
        int? expectedVersion = null,
        CancellationToken cancellationToken = default)
        => await SaveAsync(
            aggregate: aggregate,
            actor: causation.Actor,
            causation: causation.Causation,
            correlation: causation.Correlation,
            scheduledPublication: causation.ScheduledPublication,
            expectedVersion: expectedVersion,
            cancellationToken: cancellationToken);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task SaveAsync(
        TAggregate aggregate,
        IEventContext<IEvent> causation,
        CancellationToken cancellationToken = default)
        => await SaveAsync(
            aggregate: aggregate,
            causation: causation,
            expectedVersion: null,
            cancellationToken: cancellationToken);

    private async Task<bool> ResolveConflictAsync(
        TAggregate aggregate,
        int expectedVersion,
        long currentVersion,
        CancellationToken cancellationToken = default)
    {
        var exisitngEvents = await _eventStoreFactory.Create<TAggregate, TState>().GetEventsAsync(aggregate.Id, null, cancellationToken);
        exisitngEvents = exisitngEvents.OrderByDescending(e => e.Payload.Version);
        var prevEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version < expectedVersion);
        var nextEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version > expectedVersion);
        var conflictingEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version == expectedVersion);

        var action = await _conflictResolver.ResolveAsync(aggregate, prevEvent?.Payload, nextEvent?.Payload, conflictingEvent?.Payload);

        if (action == ConflictAction.Throw)
        {
            _exceptionStream.AddException(new ConcurrencyException(aggregate.Id, expectedVersion, currentVersion), cancellationToken);
            return false;
        }

        return true;
    }

    private async Task<TAggregate> RehydrateAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        var events = await _eventStoreFactory.Create<TAggregate, TState>().GetEventsBackwardsAsync(aggregate.Id, aggregate.Version, null, cancellationToken);

        if (!events.Any())
            return aggregate;

        foreach (var @event in events)
            aggregate.Apply(@event.Payload, false);

        return aggregate;
    }

    private async Task<TAggregate?> GetSnapshotAsync(StreamId streamId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var scope = _serviceScopeFactory.CreateScope();
        var snapshot = scope.ServiceProvider.GetService<IAggregateSnapshot<TAggregate, TState>>();

        if (snapshot == null)
            return null;

        return await snapshot.GetAsync(streamId, RehydrateAsync, cancellationToken);
    }

    private async Task SaveSnapshotAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var scope = _serviceScopeFactory.CreateScope();
        var snapshot = scope.ServiceProvider.GetService<IAggregateSnapshooter<TState>>();

        if (snapshot == null)
            return;

        await snapshot.SaveAsync(aggregate, cancellationToken);
    }
}
