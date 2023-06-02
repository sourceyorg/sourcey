namespace Zion.Events.Bus
{
    public interface IEventQueuePublisher
    {
        Task PublishAsync<TEvent>(string queue, IEventNotification<TEvent> context, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
        Task PublishAsync(string queue, IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
