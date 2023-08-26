using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sourcey.Core.Initialization;

namespace Sourcey.Extensions;

public static class HostExtensions
{
    public static async Task InitializeSourceyAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var initializers = scope.ServiceProvider.GetServices<ISourceyInitializer>();

        if (initializers is null)
            return;

        foreach (var initializer in initializers.Where(i => !i.ParallelEnabled))
            await initializer.InitializeAsync(host);

        await Task.WhenAll(initializers.Where(i => i.ParallelEnabled).Select(i => i.InitializeAsync(host)));
    }
}
