namespace Sourcey.Events.Stores.InMemory;

internal interface IEventContextFactory
{
    IEventContext<IEvent> CreateContext(InMemoryEvent @event);
}
