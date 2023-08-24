using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Subscriptions
{
    public interface ISubscriptionClientFactory
    {
        ISubscriptionClient Create(ServiceBusSubscription subscription);
    }
}
