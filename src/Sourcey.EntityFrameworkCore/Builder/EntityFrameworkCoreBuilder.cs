using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.EntityFrameworkCore.Builder
{
    internal readonly struct EntityFrameworkCoreBuilder : IEntityFrameworkCoreBuilder
    {
        public readonly IServiceCollection Services { get; }

        public EntityFrameworkCoreBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
