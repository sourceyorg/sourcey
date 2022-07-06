using System.Collections;

namespace Zion.Events.Streams
{
    public record EventStream : IEnumerable<IEvent>
    {
        private readonly IEnumerable<IEvent> _events;

        public StreamId StreamId { get; }

        public EventStream(StreamId streamId, params IEvent[] events)
        {
            StreamId = streamId;
            _events = events;
        }

        public IEnumerator<IEvent> GetEnumerator()
            => _events.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public TEvent? GetLastOrDefault<TEvent>()
            => (TEvent?)_events?.LastOrDefault();
    }
}
