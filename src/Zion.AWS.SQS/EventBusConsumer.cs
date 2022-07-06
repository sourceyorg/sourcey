using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zion.AWS.SQS.Factories;
using Zion.AWS.SQS.Messages;
using Zion.AWS.SQS.Queues;
using Zion.Events.Bus;
using Zion.Events.Execution;

namespace Zion.AWS.SQS
{
    internal sealed class EventBusConsumer : BackgroundService, IEventBusConsumer
    {
        private readonly ILogger<EventBusConsumer> _logger;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly QueueOptions _queue;
        private readonly PeriodicTimer _timer;

        public EventBusConsumer(
            ILogger<EventBusConsumer> logger,
            IEventContextFactory eventContextFactory,
            IServiceScopeFactory serviceScopeFactory,
            IOptionsSnapshot<SQSOptions> optionsSnapshot,
            QueueOptions queue)
        {
            if(logger is null)
                throw new ArgumentNullException(nameof(logger));
            if (eventContextFactory is null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (serviceScopeFactory is null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (optionsSnapshot is null)
                throw new ArgumentNullException(nameof(optionsSnapshot));

            _logger = logger;
            _eventContextFactory = eventContextFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _queue = queue;
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(optionsSnapshot.Value.PollingInterval ?? 500));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Executing event consumer for queue: {_queue}");
            await AddOrUpdateQueue();

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                var clientFactory = scope.ServiceProvider.GetRequiredService<IClientFactory>();

                using var client = clientFactory.Create();
                var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(_queue.Name) { MaxNumberOfMessages = 1 }, stoppingToken);

                if (!response.Messages.Any())
                    continue;

                var context = _eventContextFactory.CreateContext(response.Messages.First());
                await eventDispatcher.DispatchAsync(context);
            }
        }

        private async Task AddOrUpdateQueue()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var queueManager = scope.ServiceProvider.GetRequiredService<IQueueManager>();
            await queueManager.AddOrUpdateQueueAsync(_queue);
        }
    }
}
