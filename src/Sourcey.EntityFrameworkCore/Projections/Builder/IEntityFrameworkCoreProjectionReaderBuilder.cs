using Microsoft.EntityFrameworkCore;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

public interface IEntityFrameworkCoreProjectionReaderBuilder<TProjection>
    where TProjection : class, IProjection, new()
{
    IEntityFrameworkCoreProjectionReaderBuilder<TProjection> WithContext<TProjectionContext>(Action<DbContextOptionsBuilder> dbOptions)
        where TProjectionContext : DbContext;
}
