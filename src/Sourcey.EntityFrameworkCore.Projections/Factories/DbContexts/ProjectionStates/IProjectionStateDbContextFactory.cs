using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates
{
    public interface IProjectionStateDbContextFactory : IDbContextFactory<DbContext>
    {
    }
}
