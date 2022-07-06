using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Topics
{
    public interface ITopicClientFactory
    {
        Task<ITopicClient> CreateAsync();
    }
}
