using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zion.Azure.ServiceBus;
using Zion.Azure.ServiceBus.Management;
using Zion.Azure.ServiceBus.Messages;
using Zion.Azure.ServiceBus.Queues;
using Zion.Azure.ServiceBus.Subscriptions;
using Zion.Azure.ServiceBus.Topics;
using Zion.Core.Builder;
using Zion.Events.Bus;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddAzureServiceBus(this IZionBuilder builder, Action<ServiceBusOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var options = new ServiceBusOptions();
            optionsAction(options);

            foreach(var sub in options.Subscriptions)
                builder.RegisterEventCache(sub.Events.ToArray());

            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<IEventBusPublisher, AzureServiceBusPublisher>();
            builder.Services.AddSingleton<IEventBusConsumer, AzureServiceBusConsumer>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<IEventBusConsumer>());
            
            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<ISubscriptionClientFactory, DefaultSubscriptionClientFactory>();
            builder.Services.AddScoped<ISubscriptionClientManager, DefaultSubscriptionClientManager>();
            builder.Services.AddScoped<ITopicClientFactory, DefaultTopicClientFactory>();
            builder.Services.AddScoped<ITopicMessageReceiver, DefaultTopicMessageReceiver>();
            builder.Services.AddScoped<IEventContextFactory, DefaultEventContextFactory>();
            builder.Services.AddScoped<ITopicMessageSender, DefaultTopicMessageSender>();
            builder.Services.AddScoped<IServiceBusManagementClient, ServiceBusManagementClient>();
            builder.Services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ServiceBusOptions>>();
                var connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.Value.ConnectionString);

                if (string.IsNullOrEmpty(connectionStringBuilder.EntityPath) && options.Value.Topic == null)
                    throw new InvalidOperationException($"Azure service bus connection string doesn't contain an entity path and 'UseTopic(...)' has not been called. Either include the entity path in the connection string or by calling 'UseTopic(...)' during startup when configuring Azure Service Bus.");

                if (options.Value.Topic != null)
                    connectionStringBuilder.EntityPath = options.Value.Topic.Name;

                if (!string.IsNullOrEmpty(connectionStringBuilder.EntityPath) && options.Value.Topic == null)
                    options.Value.UseTopic(t => t.WithName(connectionStringBuilder.EntityPath));

                return connectionStringBuilder;
            });
            builder.Services.AddScoped(sp =>
            {
                var connectionStringBuilder = sp.GetRequiredService<ServiceBusConnectionStringBuilder>();

                return new ManagementClient(connectionStringBuilder);
            });

            return builder;
        }

        public static IZionBuilder AddAzureServiceBusQueue(this IZionBuilder builder, Action<ServiceBusQueueOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var options = new ServiceBusQueueOptions();
            optionsAction(options);

            builder.Services.Configure(optionsAction);
            builder.Services.AddSingleton(options);

            if (options.EnableConsumer)
                builder.Services.AddSingleton<IHostedService>(sp =>
                    new EventQueueConsumer(options.EntityPath,
                        sp.GetRequiredService<ILogger<EventQueueConsumer>>(),
                        sp.GetRequiredService<IServiceScopeFactory>()));

            builder.Services.TryAddScoped<IEventQueuePublisher, EventQueuePublisher>();
            builder.Services.TryAddScoped<IQueueClientFactory, QueueClientFactory>();
            builder.Services.TryAddScoped<IQueueMessageReceiver, QueueMessageReceiver>();
            builder.Services.TryAddScoped<IQueueMessageSender, QueueMessageSender>();
            builder.Services.TryAddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.TryAddScoped<IEventContextFactory, DefaultEventContextFactory>();
            builder.Services.TryAddScoped<IServiceBusManagementClient, ServiceBusManagementClient>();

            return builder;
        }
    }
}
