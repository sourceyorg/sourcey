using Sourcey.Events;
using Sourcey.Events.Stores;

namespace Sourcey.Events.Stores.InMemory;

internal interface IEventContextFactory
{
    IEventContext<IEvent> CreateContext(InMemoryEvent @event);
}
