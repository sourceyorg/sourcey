using Microsoft.Extensions.DependencyInjection;

namespace Zion.Projections.Builder
{
    public interface IZionProjectionBuilder<TProjection>
        where TProjection : class, IProjection
    {
        IServiceCollection Services { get; }
        IZionProjectionBuilder<TProjection> WithManager<TProjectionManager>()
            where TProjectionManager : class, IProjectionManager<TProjection>;
        IZionProjectionBuilder<TProjection> WithDistributedCache(Func<IServiceProvider, IProjectionWriter<TProjection>[], IProjectionManager<TProjection>> factory);
    }
}
