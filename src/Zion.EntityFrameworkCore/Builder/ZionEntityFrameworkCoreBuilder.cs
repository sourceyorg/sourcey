using Microsoft.Extensions.DependencyInjection;

namespace Zion.EntityFrameworkCore.Builder
{
    internal readonly struct ZionEntityFrameworkCoreBuilder : IZionEntityFrameworkCoreBuilder
    {
        public readonly IServiceCollection Services { get; }

        public ZionEntityFrameworkCoreBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
