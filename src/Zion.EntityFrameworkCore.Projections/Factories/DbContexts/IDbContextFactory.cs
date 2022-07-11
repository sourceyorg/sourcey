using Microsoft.EntityFrameworkCore;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Factories.DbContexts
{
    public interface IDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext? Create<TProjection>()
            where TProjection : class, IProjection;
    }
}
