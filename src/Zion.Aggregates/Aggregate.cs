using Zion.Events;
using Zion.Events.Streams;

namespace Zion.Aggregates
{
    public abstract class Aggregate<TState>
            where TState : IAggregateState, new()
    {
        private readonly List<IEvent> _uncommitedEvents;
        private readonly Dictionary<Type, Action<IEvent>> _eventHandlers;
        protected readonly TState _state;

        public StreamId Id { get; protected set; }
        public int Version { get; protected set; }

        protected Aggregate(TState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _uncommitedEvents = new List<IEvent>();
            _eventHandlers = new Dictionary<Type, Action<IEvent>>();
            _state = state;
        }

        public abstract TState GetState();
        public virtual void FromHistory(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            foreach (var @event in events)
                Apply(@event, isNew: false);
        }
        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return _uncommitedEvents.AsReadOnly();
        }
        public void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            _eventHandlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }
        
        public void Apply(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            Apply(@event, isNew: true);
        }

        public void Apply(IEvent @event, bool isNew)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (_eventHandlers.TryGetValue(@event.GetType(), out var handler))
                handler(@event);

            if (isNew)
                _uncommitedEvents.Add(@event);
            else
                Version = @event.Version;
        }
    }
}
