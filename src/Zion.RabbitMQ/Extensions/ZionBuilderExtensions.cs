﻿using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Builder;
using Zion.Events.Bus;
using Zion.Events.Extensions;
using Zion.RabbitMQ.Connections;
using Zion.RabbitMQ.Management;
using Zion.RabbitMQ.Management.Api;
using Zion.RabbitMQ.Messages;
using Zion.RabbitMQ.Queues;
using Zion.RabbitMQ.Subscriptions;

namespace Zion.RabbitMQ.Extensions
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

            builder.Services.ConfigureOptions(options);

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
