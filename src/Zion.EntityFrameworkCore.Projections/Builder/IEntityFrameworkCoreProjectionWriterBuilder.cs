using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.DbContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    public interface IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
        where TProjection : class, IProjection
    {
        IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
            where TProjectionContext : DbContext;

        IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithStateManagement<TProjectionStateDbContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
            where TProjectionStateDbContext : ProjectionStateDbContextBase<TProjectionStateDbContext>;
    }
}
