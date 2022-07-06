using Zion.EntityFrameworkCore.Projections.DbContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Factories
{
    public interface IProjectionDbContextFactory
    {
        ProjectionDbContext Create<TProjection>()
            where TProjection : class, IProjection;
    }
}
