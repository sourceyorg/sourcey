using Sourcey.Core.Keys;
using Sourcey.Events.Streams;

namespace Sourcey.Events.Stores
{
    public interface IEventStore<TEventStoreContext>
        where TEventStoreContext : IEventStoreContext
    {
        Task<Page> GetEventsAsync(long offset, int? pageSize = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, int? pageSize = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, DateTimeOffset timeStamp, int? pageSize = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(StreamId streamId, long offset, int? pageSize = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsBackwardsAsync(StreamId streamId, long? version, long? position, CancellationToken cancellationToken = default);
        Task<IEventContext<IEvent>> GetEventAsync(Subject subject, CancellationToken cancellationToken = default);
        Task SaveAsync(StreamId streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default);
        Task<long> CountAsync(StreamId streamId, CancellationToken cancellationToken = default);
    }
}
