using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Queues
{
    public interface IQueueClientManager
    {
        Task<IQueueClient> RegisterClientAsync(string queue);
    }
}
