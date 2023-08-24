using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Topics
{
    public interface ITopicMessageReceiver
    {
        Task ReceiveAsync(ISubscriptionClient client, Message message, CancellationToken cancellationToken = default);
        Task OnErrorAsync(ExceptionReceivedEventArgs error);
    }
}
