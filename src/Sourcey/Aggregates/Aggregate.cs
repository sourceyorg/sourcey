using Sourcey.Events;
using Sourcey.Keys;

namespace Sourcey.Aggregates;

/// <summary>
/// Base class for all aggregates.
/// </summary>
public abstract class Aggregate<TState>
    where TState : IAggregateState, new()
{
    private readonly List<IEvent> _uncommitedEvents = new();
    private readonly Dictionary<Type, Action<IEvent>> _eventHandlers = new();

    /// <summary>
    /// The current state of the aggregate.
    /// </summary>
    protected readonly TState _state = new();

    /// <summary>
    /// The unique identifier of the aggregate.
    /// </summary>
    public StreamId Id { get; protected set; }

    /// <summary>
    /// The current version of the aggregate.
    /// </summary>
    public int? Version { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the state <see cref="TState"/>.
    /// </summary>
    public abstract TState GetState();

    /// <summary>
    /// Replays the specified events to the aggregate.
    /// <param name="events">Events to be replayed</param>
    /// </summary>
    public virtual void FromHistory(IEnumerable<IEvent> events)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        foreach (var @event in events)
            Apply(@event, isNew: false);
    }

    /// <summary>
    /// Gets the uncommitted events.
    /// <returns>The uncommitted events.</returns>
    /// </summary>
    public IEnumerable<IEvent> GetUncommittedEvents()
    {
        return _uncommitedEvents.AsReadOnly();
    }

    /// <summary>
    /// Clears the uncommitted events.
    /// </summary>
    public void ClearUncommittedEvents()
    {
        _uncommitedEvents.Clear();
    }

    /// <summary>
    /// Registers an event handler for the specified event type.
    /// <param name="handler">The event handler.</param>
    /// </summary>
    protected void Handles<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        _eventHandlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
    }

    /// <summary>
    /// Applies the specified event to the aggregate.
    /// <param name="event">The event to be applied.</param>
    /// </summary>
    public void Apply(IEvent @event)
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        Apply(@event, isNew: true);
    }


    /// <summary>
    /// Applies the specified event to the aggregate.
    /// <param name="event">The event to be applied.</param>
    /// <param name="isNew">Indicates whether the event is new or not.</param>
    /// </summary>
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
