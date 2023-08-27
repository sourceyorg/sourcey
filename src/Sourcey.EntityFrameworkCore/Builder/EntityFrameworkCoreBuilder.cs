using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.EntityFrameworkCore.Builder;

internal readonly struct EntityFrameworkCoreBuilder : IEntityFrameworkCoreBuilder
{
    private readonly IServiceCollection _services;

    public IServiceCollection Services => _services;

    public EntityFrameworkCoreBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }
}
