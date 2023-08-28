using Sourcey.Events;
using Sourcey.Events.Serialization;
using Sourcey.Events.Stores;
using Sourcey.Extensions;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.Events.Factories;

internal sealed class EventModelFactory : IEventModelFactory
{
    private readonly IEventSerializer _eventSerializer;

    public EventModelFactory(IEventSerializer eventSerializer)
    {
        if (eventSerializer == null)
            throw new ArgumentNullException(nameof(eventSerializer));

        _eventSerializer = eventSerializer;
    }

    public Entities.Event Create(StreamId streamId, IEventContext<IEvent> context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (context.Payload == null)
            throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

        var @event = context.Payload;
        var type = @event.GetType();

        return new Entities.Event
        {
            StreamId = streamId,
            Correlation = context.Correlation,
            Causation = context.Causation,
            Data = _eventSerializer.Serialize(@event),
            Id = @event.Id,
            Name = type.Name,
            Type = type.FriendlyName(),
            Timestamp = @event.Timestamp,
            Actor = context.Actor,
            ScheduledPublication = context.ScheduledPublication,
            Version = @event.Version
        };
    }
}
