using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Queues
{
    public interface IQueueClientManager
    {
        Task<IQueueClient> RegisterClientAsync(string queue);
    }
}
