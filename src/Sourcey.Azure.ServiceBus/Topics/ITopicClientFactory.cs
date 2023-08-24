using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Topics
{
    public interface ITopicClientFactory
    {
        Task<ITopicClient> CreateAsync();
    }
}
