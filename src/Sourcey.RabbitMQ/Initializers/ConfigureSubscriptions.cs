using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sourcey.Core.Initialization;
using Sourcey.RabbitMQ.Subscriptions;

namespace Sourcey.RabbitMQ.Initializers
{
    internal class ConfigureSubscriptions : ISourceyInitializer
    {
        public bool ParallelEnabled => false;

        public async Task InitializeAsync(IHost host)
        {
            using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
            await manager.ConfigureAsync();
        }
    }
}
