using System.Collections.Concurrent;
using Sourcey.Events.Cache;
using Sourcey.Events.Streams;
using Sourcey.Keys;

namespace Sourcey.Events.Stores.InMemory;


internal sealed class InMemoryContext : IEventStoreContext
{
    // public void Dispose()
    // {
    //     return;
    // }

    // public ValueTask DisposeAsync()
    // {
    //     return new();
    // }
}

internal sealed record InMemoryEvent(
        StreamId StreamId,
        EventId Id,
        Correlation? Correlation,
        Causation? Causation,
        string Data,
        string Name,
        string Type,
        DateTimeOffset Timestamp,
        Actor Actor,
        DateTimeOffset? ScheduledPublication,
        int? Version,
        long SequenceNo
    );

internal sealed class InMemoryStore
{
    internal readonly ConcurrentBag<InMemoryEvent> _events = new();
}

internal sealed class InMemoryEventStore : IEventStore<InMemoryContext>
{
    private readonly InMemoryStore _store;
    private readonly IEventTypeCache _eventTypeCache;
    private readonly IEventContextFactory _eventContextFactory;
    private readonly IEventModelFactory _eventModelFactory;
    private readonly IEventStreamManager _eventStreamManager;

    public InMemoryEventStore(InMemoryStore store, IEventTypeCache eventTypeCache, IEventContextFactory eventContextFactory, IEventModelFactory eventModelFactory, IEventStreamManager eventStreamManager)
    {
        _store = store;
        _eventTypeCache = eventTypeCache;
        _eventContextFactory = eventContextFactory;
        _eventModelFactory = eventModelFactory;
        _eventStreamManager = eventStreamManager;
    }

    public Task<long> CountAsync(StreamId streamId, CancellationToken cancellationToken = default)
        => Task.FromResult(_store._events.LongCount(@event => @event.StreamId == streamId));

    public async Task<Page> GetEventsAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var results = new List<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>>();

        var events = await GetAllEventsForwardsInternalAsync(offset, pageSize, cancellationToken).ConfigureAwait(false);

        var eventsByStreamId = events.GroupBy(@event => @event.StreamId)
            .Select(g => GetValuesAsync(g.Key, g.ToArray()))
            .ToArray();

        await Task.WhenAll(eventsByStreamId);

        foreach (var task in eventsByStreamId)
            results.Add(await task);


        var sequenceNumbers = events.Select(e => e.SequenceNo);

        return new Page(sequenceNumbers.Any() ? sequenceNumbers.Max() + 1 : offset, offset, results);
    }

    private Task<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>> GetValuesAsync(StreamId streamId, IEnumerable<InMemoryEvent> events)
    {
        var results = new List<IEventContext<IEvent>>();

        foreach (var @event in events.OrderBy(e => e.SequenceNo))
        {
            if (@event.Type is null || !_eventTypeCache.TryGet(@event.Type, out var type))
                continue;

            var result = _eventContextFactory.CreateContext(@event);

            results.Add(result);
        }

        return Task.FromResult(new KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>(streamId, results));
    }

    public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var results = new List<IEventContext<IEvent>>();

        var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, 0, pageSize, cancellationToken).ConfigureAwait(false);

        foreach (var @event in events)
        {
            if (@event.Type is null || !_eventTypeCache.TryGet(@event.Type, out var type))
                continue;

            var result = _eventContextFactory.CreateContext(@event);

            results.Add(result);
        }

        return results;

    }
    public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, long offset, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var results = new List<IEventContext<IEvent>>();

        var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, offset, pageSize, cancellationToken).ConfigureAwait(false);

        foreach (var @event in events)
        {
            if (@event.Type is null || !_eventTypeCache.TryGet(@event.Type, out _))
                continue;

            var result = _eventContextFactory.CreateContext(@event);

            results.Add(result);
        }

        return results;
    }

    public Task<IEventContext<IEvent>> GetEventAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        var @event = _store._events.FirstOrDefault(e => e.Id == subject);

        if (@event == null)
            throw new InvalidOperationException($"Unable to find event: {subject}");

        if (!_eventTypeCache.TryGet(@event.Type, out _))
            throw new InvalidOperationException($"Unable to find event type: {@event.Type}");

        return Task.FromResult(_eventContextFactory.CreateContext(@event));
    }

    public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, DateTimeOffset timeStamp, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var results = new List<IEventContext<IEvent>>();

        var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, timeStamp, pageSize, cancellationToken).ConfigureAwait(false);

        foreach (var @event in events)
        {
            if (@event.Type is null || !_eventTypeCache.TryGet(@event.Type, out var type))
                throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

            var result = _eventContextFactory.CreateContext(@event);

            results.Add(result);
        }

        return results;
    }

    public Task SaveAsync(StreamId streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        foreach (var @event in events)
            _store._events.Add(_eventModelFactory.Create(streamId, @event, _store._events.Count));

        _eventStreamManager.Append(events.ToArray());

        return Task.CompletedTask;
    }

    public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsBackwardsAsync(StreamId streamId, long? version, long? position, CancellationToken cancellationToken = default)
    {
        var results = new List<IEventContext<IEvent>>();

        var events = await GetEventsBackwardsInternalAsync(streamId, version, position, cancellationToken).ConfigureAwait(false);

        foreach (var @event in events.OrderBy(e => e.SequenceNo))
        {
            if (@event.Type is null || !_eventTypeCache.TryGet(@event.Type, out var type))
                throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

            var result = _eventContextFactory.CreateContext(@event);

            results.Add(result);
        }

        return results;
    }

    private Task<List<InMemoryEvent>> GetEventsBackwardsInternalAsync(StreamId streamId, long? version, long? position, CancellationToken cancellationToken = default)
    {
        var events = _store._events
            .OrderByDescending(e => e.SequenceNo)
            .Where(e => e.StreamId == streamId);

        if (version.HasValue)
            events = events.Where(e => e.Version > version);

        if (position.HasValue)
            events = events.Where(e => e.SequenceNo <= position.Value);

        return Task.FromResult(events.ToList());
    }

    private Task<List<InMemoryEvent>> GetAllEventsForwardsInternalAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var events = _store._events
            .OrderBy(e => e.SequenceNo)
            .Where(e => e.SequenceNo >= offset);


        if (pageSize.HasValue)
            events = events.Take(pageSize.Value);

        return Task.FromResult(events.ToList());
    }

    private Task<List<InMemoryEvent>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, DateTimeOffset timeStamp, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var events = _store._events
            .OrderBy(e => e.SequenceNo)
            .Where(e => e.Timestamp >= timeStamp)
            .Where(e => e.StreamId == streamId);


        if (pageSize.HasValue)
            events = events.Take(pageSize.Value);

        return Task.FromResult(events.ToList());
    }

    private Task<List<InMemoryEvent>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, long offset, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        var events = _store._events
            .OrderBy(e => e.SequenceNo)
            .Where(e => e.SequenceNo >= offset)
            .Where(e => e.StreamId == streamId);

        if (pageSize.HasValue)
            events = events.Take(pageSize.Value);

        return Task.FromResult(events.ToList());
    }
}