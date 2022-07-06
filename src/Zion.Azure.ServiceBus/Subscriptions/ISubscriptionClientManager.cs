using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientManager
    {
        Task<IReadOnlyList<ISubscriptionClient>> RegisterClientsAsync();
        Task ConfigureSubscriptionsAsync();
    }
}
