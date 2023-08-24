using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Core.Builder;
using Sourcey.Core.Initialization;
using Sourcey.Events.Bus;
using Sourcey.RabbitMQ;
using Sourcey.RabbitMQ.Connections;
using Sourcey.RabbitMQ.Initializers;
using Sourcey.RabbitMQ.Management;
using Sourcey.RabbitMQ.Management.Api;
using Sourcey.RabbitMQ.Messages;
using Sourcey.RabbitMQ.Queues;
using Sourcey.RabbitMQ.Subscriptions;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddRabbitMq(this ISourceyBuilder builder, Action<RabbitMqOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var options = new RabbitMqOptions();
            optionsAction(options);

            foreach (var sub in options.Subscriptions)
                builder.RegisterEventCache(sub.Events.ToArray());

            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<ISourceyInitializer, ConfigureSubscriptions>();
            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<IEventBusPublisher, RabbitMqEventBusPublisher>();
            builder.Services.AddSingleton<IEventBusConsumer, RabbitMqEventBusConsumer>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<IEventBusConsumer>());
            builder.Services.AddSingleton<RabbitMqConnectionPool>();
            builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            builder.Services.AddScoped<IQueueMessageSender, DefaultQueueMessageSender>();
            builder.Services.AddSingleton<IQueueMessageReceiver, DefaultQueueMessageReceiver>();
            builder.Services.AddScoped<ISubscriptionManager, DefaultSubscriptionManager>();
            builder.Services.AddScoped<IRabbitMqManagementClient, RabbitMqManagementClient>();
            builder.Services.AddHttpClient<IRabbitMqManagementApiClient, RabbitMqManagementApiClient>();
            builder.Services.AddSingleton<IEventContextFactory, DefaultEventContextFactory>();

            return builder;
        }
    }
}
