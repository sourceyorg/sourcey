using Microsoft.EntityFrameworkCore;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;

public interface IDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    public TDbContext? Create<TProjection>()
        where TProjection : class, IProjection;
}
