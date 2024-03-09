using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

public interface IEntityFrameworkCoreStateManagementBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    IEntityFrameworkCoreStateManagementBuilder<TProjection> WithContext<TProjectionContext>(bool autoMigrate = true)
        where TProjectionContext : ProjectionStateDbContext;
}
