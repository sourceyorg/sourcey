using Sourcey.Events;
using Sourcey.Events.Stores;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.Events.Factories;

public interface IEventModelFactory
{
    Entities.Event Create(StreamId streamId, IEventContext<IEvent> context);
}
