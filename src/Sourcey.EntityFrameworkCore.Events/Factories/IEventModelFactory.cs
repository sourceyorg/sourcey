using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Events.Streams;

namespace Sourcey.EntityFrameworkCore.Events.Factories;

public interface IEventModelFactory
{
    Entities.Event Create(StreamId streamId, IEventContext<IEvent> context);
}
