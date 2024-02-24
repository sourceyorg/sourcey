using Microsoft.EntityFrameworkCore;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

public interface IEntityFrameworkCoreProjectionWriterBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    IEntityFrameworkCoreProjectionWriterBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions, bool autoMigrate = true)
        where TProjectionContext : DbContext;
}
