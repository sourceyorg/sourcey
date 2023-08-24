using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sourcey.Projections.Builder
{
    internal readonly struct ProjectionBuilder<TProjection> : IProjectionBuilder<TProjection>
        where TProjection : class, IProjection
    {
        public readonly IServiceCollection Services { get; }

        public ProjectionBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            Services = services;
        }

        public IProjectionBuilder<TProjection> WithManager<TProjectionManager>() where TProjectionManager : class, IProjectionManager<TProjection>
        {
            Services.TryAddScoped<IProjectionManager<TProjection>, TProjectionManager>();
            return this;
        }
    }
}
