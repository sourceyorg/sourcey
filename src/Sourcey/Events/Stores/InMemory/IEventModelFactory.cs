using Sourcey.Keys;

namespace Sourcey.Events.Stores.InMemory;

internal interface IEventModelFactory
{
    InMemoryEvent Create(StreamId streamId, IEventContext<IEvent> context, int count);
}
