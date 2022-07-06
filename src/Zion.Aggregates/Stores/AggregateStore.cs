using Zion.Aggregates.Concurrency;
using Zion.Commands;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.Aggregates.Stores
{
    public sealed class AggregateStore<TEventStoreContext> : IAggregateStore<TEventStoreContext>
        where TEventStoreContext : IEventStoreContext
    {
        private readonly IEventStore<TEventStoreContext> _eventStore;
        private readonly IAggregateFactory _aggregateFactory;
        private readonly IConflictResolver _conflictResolver;

        public AggregateStore(IEventStore<TEventStoreContext> eventStore,
            IAggregateFactory aggregateFactory,
            IConflictResolver conflictResolver)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));
            if (conflictResolver == null)
                throw new ArgumentNullException(nameof(conflictResolver));

            _eventStore = eventStore;
            _aggregateFactory = aggregateFactory;
            _conflictResolver = conflictResolver;
        }

        public async Task<TAggregate> GetAsync<TAggregate, TState>(StreamId id, CancellationToken cancellationToken = default)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            var events = await _eventStore.GetEventsAsync(id, cancellationToken);

            if (!events.Any())
                return default;

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

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                if (await ResolveConflictAsync(aggregate, expectedVersion.Value, cancellationToken) == ConflictAction.Throw)
                    throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

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
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null, CancellationToken cancellationToken = default)
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

            var currentVersion = await _eventStore.CountAsync(StreamId.From(aggregate.Id));

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                if (await ResolveConflictAsync(aggregate, expectedVersion.Value, cancellationToken) == ConflictAction.Throw)
                    throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            events = aggregate.GetUncommittedEvents();

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlation: causation.Correlation,
                causation: Causation.From(causation.Id),
                timestamp: @event.Timestamp,
                actor: causation.Actor,
                scheduledPublication: null));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts);

            aggregate.ClearUncommittedEvents();
        }
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

            var currentVersion = await _eventStore.CountAsync(StreamId.From(aggregate.Id));

            if (expectedVersion.HasValue && expectedVersion.Value != currentVersion)
                if (await ResolveConflictAsync(aggregate, expectedVersion.Value, cancellationToken) == ConflictAction.Throw)
                    throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            events = aggregate.GetUncommittedEvents();

            var contexts = events.Select(@event => new EventContext<IEvent>(
                streamId: aggregate.Id,
                @event: @event,
                correlation: causation.Correlation,
                causation: Causation.From(causation.Payload.Id),
                timestamp: @event.Timestamp,
                actor: causation.Actor,
                scheduledPublication: causation.ScheduledPublication));

            await _eventStore.SaveAsync(StreamId.From(aggregate.Id), contexts);

            aggregate.ClearUncommittedEvents();
        }

        private async Task<ConflictAction> ResolveConflictAsync<TState>(Aggregate<TState> aggregate, int expectedVersion, CancellationToken cancellationToken = default)
            where TState : IAggregateState, new()
        {
            var exisitngEvents = await _eventStore.GetEventsAsync(aggregate.Id, cancellationToken);
            exisitngEvents = exisitngEvents.OrderByDescending(e => e.Payload.Version);
            var prevEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version < expectedVersion);
            var nextEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version > expectedVersion);
            var conflictingEvent = exisitngEvents.FirstOrDefault(e => e.Payload.Version == expectedVersion);

            return await _conflictResolver.ResolveAsync(aggregate, prevEvent?.Payload, nextEvent?.Payload, conflictingEvent?.Payload);
        }
    }
}
