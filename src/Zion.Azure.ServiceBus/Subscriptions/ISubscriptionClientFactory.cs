using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create(ServiceBusSubscription subscription);
    }
}
