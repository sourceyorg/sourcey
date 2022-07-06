using Zion.Events;
using Zion.Events.Stores;
using Zion.Events.Streams;

namespace Zion.EntityFrameworkCore.Events.Factories
{
    public interface IEventModelFactory
    {
        Entities.Event Create(StreamId streamId, IEventContext<IEvent> context);
    }
}
