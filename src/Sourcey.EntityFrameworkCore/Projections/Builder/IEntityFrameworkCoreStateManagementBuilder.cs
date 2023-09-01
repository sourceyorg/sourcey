using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

public interface IEntityFrameworkCoreStateManagementBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    IEntityFrameworkCoreStateManagementBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionContext : ProjectionStateDbContext;
}