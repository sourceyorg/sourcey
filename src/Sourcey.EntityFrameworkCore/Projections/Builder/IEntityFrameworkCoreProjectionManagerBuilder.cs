using Microsoft.EntityFrameworkCore;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

public interface IEntityFrameworkCoreProjectionManagerBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    IEntityFrameworkCoreProjectionManagerBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionContext : DbContext;
}