using Microsoft.Extensions.DependencyInjection;

namespace Zion.Core.Builder
{
    internal readonly struct ZionBuilder : IZionBuilder
    {
        public readonly IServiceCollection Services { get; }

        public ZionBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }
    }
}
