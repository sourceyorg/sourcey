using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Queues
{
    public interface IQueueClientFactory
    {
        IQueueClient Create(ServiceBusQueueOptions options);
        IQueueClient Create(string queue);
    }
}
