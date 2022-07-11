using Zion.EntityFrameworkCore.Projections.DbContexts;

namespace Zion.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates
{
    internal sealed class ProjectionStateDbContextFactory : DbContextFactory<ProjectionStateDbContext, ProjectionStateDbType>, IProjectionStateDbContextFactory
    {
        public ProjectionStateDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<ProjectionStateDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
        {
        }
    }
}
