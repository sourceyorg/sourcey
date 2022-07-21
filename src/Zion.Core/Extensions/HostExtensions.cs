using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;

namespace Zion.Core.Extensions
{
    public static class HostExtensions
    {
        public static async Task InitializeZionAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var initializers = scope.ServiceProvider.GetServices<IZionInitializer>();

            if (initializers is null)
                return;

            foreach (var initializer in initializers.Where(i => !i.ParallelEnabled))
                await initializer.InitializeAsync(host);

            await Task.WhenAll(initializers.Where(i => i.ParallelEnabled).Select(i => i.InitializeAsync(host)));
        }
    }
}
