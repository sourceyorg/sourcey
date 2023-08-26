using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Projections.Builder;

public interface IProjectionBuilder<TProjection>
    where TProjection : class, IProjection
{
    IServiceCollection Services { get; }
    IProjectionBuilder<TProjection> WithManager<TProjectionManager>()
        where TProjectionManager : class, IProjectionManager<TProjection>;
}
