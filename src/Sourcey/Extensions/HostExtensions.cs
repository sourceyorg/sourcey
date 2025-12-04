using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sourcey.Initialization;

namespace Sourcey.Extensions;

public static class HostExtensions
{
    public static async Task InitializeSourceyAsync(this IHost host)
    {
        var scope = host.Services.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var initializers = scope.ServiceProvider.GetServices<ISourceyInitializer>();

            if (initializers is null)
                return;

            foreach (var initializer in initializers.Where(i => !i.ParallelEnabled))
                await initializer.InitializeAsync(host).ConfigureAwait(false);

            await Task.WhenAll(initializers.Where(i => i.ParallelEnabled).Select(i => i.InitializeAsync(host)))
                .ConfigureAwait(false);
        }
    }
}
