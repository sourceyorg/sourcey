using Sourcey.Events.Serialization;
using Sourcey.Extensions;
using Sourcey.Keys;

namespace Sourcey.Events.Stores.InMemory;

internal sealed class EventModelFactory : IEventModelFactory
{
    private readonly IEventSerializer _eventSerializer;

    public EventModelFactory(IEventSerializer eventSerializer)
    {
        if (eventSerializer == null)
            throw new ArgumentNullException(nameof(eventSerializer));

        _eventSerializer = eventSerializer;
    }

    public InMemoryEvent Create(StreamId streamId, IEventContext<IEvent> context, int count)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (context.Payload == null)
            throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

        var @event = context.Payload;
        var type = @event.GetType();

        return new InMemoryEvent(
            streamId,
            @event.Id,
            context.Correlation,
            context.Causation,
            _eventSerializer.Serialize(@event),
            type.Name,
            type.FriendlyName(),
            @event.Timestamp,
            context.Actor,
            context.ScheduledPublication,
            @event.Version,
            count + 1
        );
    }
}
