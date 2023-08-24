using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientManager
    {
        Task<IReadOnlyList<ISubscriptionClient>> RegisterClientsAsync();
        Task ConfigureSubscriptionsAsync();
    }
}
