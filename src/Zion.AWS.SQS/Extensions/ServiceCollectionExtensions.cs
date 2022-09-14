using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zion.AWS.SQS;
using Zion.AWS.SQS.Factories;
using Zion.AWS.SQS.Messages;
using Zion.AWS.SQS.Queues;
using Zion.Events.Bus;

namespace Zion.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAWSSQS(this IServiceCollection services, Action<SQSOptions> optionsAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = new SQSOptions();
            optionsAction(options);

            services.ConfigureOptions(options);

            foreach (var queue in options.SQSReadQueues)
            {
                services.AddHostedService(s => new EventBusConsumer(
                        logger: s.GetRequiredService<ILogger<EventBusConsumer>>(),
                        eventContextFactory: s.GetRequiredService<IEventContextFactory>(),
                        serviceScopeFactory: s.GetRequiredService<IServiceScopeFactory>(),
                        optionsSnapshot: s.GetRequiredService<IOptionsSnapshot<SQSOptions>>(),
                        queue: queue
                    )
                );
            }

            services.TryAddScoped<IEventBusPublisher, EventBusPublisher>();
            services.TryAddScoped<IClientFactory, ClientFactory>();
            services.TryAddScoped<IQueueManager, QueueManager>();
            services.TryAddSingleton<IEventContextFactory, EventContextFactory>();
            services.TryAddSingleton<IMessageFactory, MessageFactory>();

            return services;
        }
    }
}
