using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.RabbitMQ.Subscriptions;

namespace Zion.RabbitMQ.Initializers
{
    internal class ConfigureSubscriptions : IZionInitializer
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
