using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Builder;
using Zion.Core.Initialization;
using Zion.Events.Bus;
using Zion.RabbitMQ;
using Zion.RabbitMQ.Connections;
using Zion.RabbitMQ.Initializers;
using Zion.RabbitMQ.Management;
using Zion.RabbitMQ.Management.Api;
using Zion.RabbitMQ.Messages;
using Zion.RabbitMQ.Queues;
using Zion.RabbitMQ.Subscriptions;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddRabbitMq(this IZionBuilder builder, Action<RabbitMqOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var options = new RabbitMqOptions();
            optionsAction(options);

            foreach (var sub in options.Subscriptions)
                builder.RegisterEventCache(sub.Events.ToArray());

            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<IZionInitializer, ConfigureSubscriptions>();
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
