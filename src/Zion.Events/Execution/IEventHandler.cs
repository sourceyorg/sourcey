using Zion.Events.Stores;

namespace Zion.Events.Execution
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken = default);
    }
}
