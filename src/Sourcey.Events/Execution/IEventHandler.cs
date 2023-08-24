using Sourcey.Events.Stores;

namespace Sourcey.Events.Execution
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken = default);
    }
}
