using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts
{
    internal sealed class ProjectionDbContextFactory : DbContextFactory<DbContext, ProjectionDbType>, IProjectionDbContextFactory
    {
        public ProjectionDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<ProjectionDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
        {
        }
    }
}
