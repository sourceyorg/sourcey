﻿using Microsoft.Azure.ServiceBus;

namespace Sourcey.Azure.ServiceBus.Subscriptions
{
    internal sealed class DefaultSubscriptionClientFactory : ISubscriptionClientFactory
    {
        private readonly ServiceBusConnectionStringBuilder _connectionStringBuilder;

        public DefaultSubscriptionClientFactory(ServiceBusConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder == null)
                throw new ArgumentNullException(nameof(connectionStringBuilder));

            _connectionStringBuilder = connectionStringBuilder;
        }

        public ISubscriptionClient Create(ServiceBusSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var client = new SubscriptionClient(
                connectionStringBuilder: _connectionStringBuilder,
                subscriptionName: subscription.Name);

            return client;
        }
    }
}
