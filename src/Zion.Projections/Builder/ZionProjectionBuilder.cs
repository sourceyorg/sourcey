using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Zion.Projections.Builder
{
    internal readonly struct ZionProjectionBuilder<TProjection> : IZionProjectionBuilder<TProjection>
        where TProjection : class, IProjection
    {
        public readonly IServiceCollection Services { get; }

        public ZionProjectionBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }

        public IZionProjectionBuilder<TProjection> WithManager<TProjectionManager>() where TProjectionManager : class, IProjectionManager<TProjection>
        {
            Services.TryAddScoped<IProjectionManager<TProjection>, TProjectionManager>();
            return this;
        }
    }
}
