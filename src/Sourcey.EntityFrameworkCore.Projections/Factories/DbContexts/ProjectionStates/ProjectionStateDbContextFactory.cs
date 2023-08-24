using Microsoft.EntityFrameworkCore;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates
{
    internal sealed class ProjectionStateDbContextFactory : DbContextFactory<DbContext, ProjectionStateDbType>, IProjectionStateDbContextFactory
    {
        public ProjectionStateDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<ProjectionStateDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
        {
        }
    }
}
