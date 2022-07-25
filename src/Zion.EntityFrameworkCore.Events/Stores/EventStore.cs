using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zion.Core.Keys;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.EntityFrameworkCore.Events.Factories;
using Zion.Events;
using Zion.Events.Cache;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.EntityFrameworkCore.Events.Stores
{
    internal sealed class EventStore<TStoreDbContext> : IEventStore<TStoreDbContext>
        where TStoreDbContext : DbContext, IEventStoreDbContext
    {
        private static readonly int DefaultReloadInterval = 2000;
        private static readonly int DefaultPageSize = 500;

        private readonly IEventStoreDbContextFactory<TStoreDbContext> _dbContextFactory;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IEventModelFactory _eventModelFactory;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly ILogger<EventStore<TStoreDbContext>> _logger;
        private readonly IEventStreamManager _eventStreamManager;

        public EventStore(IEventStoreDbContextFactory<TStoreDbContext> dbContextFactory,
                                             IEventContextFactory eventContextFactory,
                                             IEventModelFactory eventModelFactory,
                                             IEventTypeCache eventTypeCache,
                                             ILogger<EventStore<TStoreDbContext>> logger,
                                             IEventStreamManager eventStreamManager)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (eventContextFactory == null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (eventModelFactory == null)
                throw new ArgumentNullException(nameof(eventModelFactory));
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (eventStreamManager == null)
                throw new ArgumentNullException(nameof(eventStreamManager));

            _dbContextFactory = dbContextFactory;
            _eventContextFactory = eventContextFactory;
            _eventModelFactory = eventModelFactory;
            _eventTypeCache = eventTypeCache;
            _logger = logger;
            _eventStreamManager = eventStreamManager;
        }

        public async Task<long> CountAsync(StreamId streamId, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.Create();
            return await context.Events.LongCountAsync(@event => @event.StreamId == streamId, cancellationToken);
        }

        public async Task<Page> GetEventsAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            var results = new List<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>>();

            var events = await GetAllEventsForwardsInternalAsync(offset, pageSize, cancellationToken).ConfigureAwait(false);

            if (events.Count > 0 && events[0].SequenceNo != offset + 1)
            {
                _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", offset + 1, events[0].SequenceNo, DefaultReloadInterval);
                events = await GetAllEventsAfterDelayInternalAsync(offset, pageSize, cancellationToken).ConfigureAwait(false);
            }

            for (var i = 0; i < events.Count - 1; i++)
            {
                if (events[i].SequenceNo + 1 != events[i + 1].SequenceNo)
                {
                    _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", events[i].SequenceNo + 1, events[i + 1].SequenceNo, DefaultReloadInterval);
                    events = await GetAllEventsAfterDelayInternalAsync(offset, pageSize, cancellationToken).ConfigureAwait(false);
                    break;
                }
            }

            var eventsByStreamId = events.GroupBy(@event => @event.StreamId)
                .Select(g => GetValuesAsync(g.Key, g.ToArray()))
                .ToArray();
            
            await Task.WhenAll(eventsByStreamId);

            foreach (var task in eventsByStreamId)
                results.Add(await task);

            return new Page(offset + events.Count, offset, results);
        }

        private Task<KeyValuePair<StreamId, IEnumerable<IEventContext<IEvent>>>> GetValuesAsync(StreamId streamId, IEnumerable<Entities.Event> events)
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

        public async Task<IEventContext<IEvent>> GetEventAsync(Subject subject, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var @event = await context.Events
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == subject, cancellationToken);

                if (@event == null)
                    throw new InvalidOperationException($"Unable to find event: {subject}");

                if (!_eventTypeCache.TryGet(@event.Type, out _))
                    throw new InvalidOperationException($"Unable to find event type: {@event.Type}");

                return _eventContextFactory.CreateContext(@event);
            }
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

        public async Task SaveAsync(StreamId streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(streamId, @event));

                await context.SaveChangesAsync(cancellationToken);
            }

            _eventStreamManager.Append(events.ToArray());
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsInternalAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.Create();

            var events = context.Events
                .AsNoTracking()
                .OrderBy(e => e.SequenceNo)
                .Where(e => e.SequenceNo >= offset);


            if (pageSize.HasValue)
                events = events.Take(pageSize.Value);

            return await events.ToListAsync(cancellationToken);
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, DateTimeOffset timeStamp, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.Create();
            
            var events = context.Events
                .AsNoTracking()
                .OrderBy(e => e.SequenceNo)
                .Where(e => e.Timestamp >= timeStamp)
                .Where(e => e.StreamId == streamId);


            if (pageSize.HasValue)
                events = events.Take(pageSize.Value);

            return await events.ToListAsync(cancellationToken);
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsForStreamInternalAsync(StreamId streamId, long offset, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            using var context = _dbContextFactory.Create();
            
            var events = context.Events
                .AsNoTracking()
                .OrderBy(e => e.SequenceNo)
                .Where(e => e.SequenceNo >= offset)
                .Where(e => e.StreamId == streamId);
                
            if (pageSize.HasValue)
                events = events.Take(pageSize.Value);

            return await events.ToListAsync(cancellationToken);
        }
        private async Task<List<Entities.Event>> GetAllEventsAfterDelayInternalAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Reloading events after {DefaultReloadInterval}ms.", DefaultReloadInterval);

            await Task.Delay(DefaultReloadInterval).ConfigureAwait(false);
            return await GetAllEventsForwardsInternalAsync(offset, pageSize, cancellationToken).ConfigureAwait(false);
        }
    }
}
