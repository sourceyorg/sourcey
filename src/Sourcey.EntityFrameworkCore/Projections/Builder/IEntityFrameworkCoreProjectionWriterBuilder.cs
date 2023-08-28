using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Builder;

public interface IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
    where TProjection : class, IProjection
{
    IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionContext : DbContext;

    IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithStateManagement<TProjectionStateDbContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionStateDbContext : ProjectionStateDbContext;
}
