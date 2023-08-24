using Sourcey.RabbitMQ.Messages;

namespace Sourcey.RabbitMQ.Queues
{
    public interface IQueueMessageReceiver
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task OnReceiveAsync(ReceivedMessage message, CancellationToken cancellationToken);
        Task OnErrorAsync(ReceivedMessage message, Exception ex);
    }
}
