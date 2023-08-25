using Microsoft.Extensions.DependencyInjection;
using Sourcey.Aggregates.Concurrency;
using Sourcey.Aggregates.Snapshots;
using Sourcey.Core.Exceptions;
using Sourcey.Core.Keys;
using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Events.Streams;

namespace Sourcey.Aggregates.Stores
{
    internal sealed class AggregateStore<TEventStoreContext> : IAggregateStore<TEventStoreContext>
        where TEventStoreContext : IEventStoreContext
    {
        private readonly IEventStore<TEventStoreContext> _eventStore;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly IConflictResolver _conflictResolver;
        private readonly IExceptionStream _exceptionStream;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AggregateStore(IEventStore<TEventStoreContext> eventStore,
            IAggregateFactory aggregateFactory,
            IConflictResolver conflictResolver,
            IExceptionStream exceptionStream,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));
            if (conflictResolver == null)
                throw new ArgumentNullException(nameof(conflictResolver));
            if (exceptionStream == null)
                throw new ArgumentNullException(nameof(exceptionStream));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _eventStore = eventStore;
            _aggregateFactory = aggregateFactory;
            _conflictResolver = conflictResolver;
            _exceptionStream = exceptionStream;
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public async Task<TAggregate?> GetAsync<TAggregate, TState>(StreamId id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            var snapshot = await GetSnapshotAsync<TAggregate, TState>(id, cancellationToken);

            if (snapshot is not null)
                return snapshot;

            var events = await _eventStore.GetEventsAsync(id, null, cancellationToken);

            if (!events.Any())
                return null;

            var aggregate = _aggregateFactory.FromHistory<TAggregate, TState>(events.Select(e => e.Payload));

            return aggregate;
        }
        
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            cancellationToken.ThrowIfCancellationRequested();

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(aggregate.Id, cancellationToken);

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion
                && !await ResolveConflictAsync(aggregate, expectedVersion.Value, currentVersion, cancellationToken))
                return;

            events = aggregate.GetUncommittedEvents();

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlation: null,
                causation: null,
                timestamp: @event.Timestamp,
                actor: Actor.From("unknown"),
                scheduledPublication: null));

            await _eventStore.SaveAsync(aggregate.Id, contexts, cancellationToken);

            aggregate.ClearUncommittedEvents();

            await SaveSnapshotAsync(aggregate, cancellationToken);
        }

        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
            => await SaveAsync(aggregate, expectedVersion: null, cancellationToken);

        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            cancellationToken.ThrowIfCancellationRequested();

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(aggregate.Id);

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion
                && !await ResolveConflictAsync(aggregate, expectedVersion.Value, currentVersion, cancellationToken))
                return;

            events = aggregate.GetUncommittedEvents();

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlation: causation.Correlation,
                causation: Causation.From(causation.Payload.Id),
                timestamp: @event.Timestamp,
                actor: causation.Actor,
                scheduledPublication: causation.ScheduledPublication));

            await _eventStore.SaveAsync(aggregate.Id, contexts);

            aggregate.ClearUncommittedEvents();

            await SaveSnapshotAsync(aggregate, cancellationToken);
        }

        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
            => await SaveAsync(aggregate, causation, null, cancellationToken);

        private async Task<bool> ResolveConflictAsync<TState>(Aggregate<TState> aggregate, int expectedVersion, long currentVersion, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            var exisitngEvents = await _eventStore.GetEventsAsync(aggregate.Id, null, cancellationToken);
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

        private async Task<TAggregate> RehydrateAsync<TAggregate, TState>(TAggregate aggregate, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            var events = await _eventStore.GetEventsBackwardsAsync(aggregate.Id, aggregate.Version, null, cancellationToken);

            if (!events.Any())
                return aggregate;

            foreach (var @event in events)
                aggregate.Apply(@event.Payload, false);

            return aggregate;
        }

        private async Task<TAggregate?> GetSnapshotAsync<TAggregate, TState>(StreamId streamId, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var scope = _serviceScopeFactory.CreateScope();
            var snapshot = scope.ServiceProvider.GetService<IAggregateSnapshot<TAggregate, TState>>();

            if (snapshot == null)
                return null;

            return await snapshot.GetAsync(streamId, RehydrateAsync<TAggregate, TState>, cancellationToken);
        }

        private async Task SaveSnapshotAsync<TState>(Aggregate<TState> aggregate, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var scope = _serviceScopeFactory.CreateScope();
            var snapshot = scope.ServiceProvider.GetService<IAggregateSnapshooter<TState>>();

            if (snapshot == null)
                return;

            await snapshot.SaveAsync(aggregate, cancellationToken);
        }
    }
}
