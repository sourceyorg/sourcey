using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.Factories.DbContexts;

namespace Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts
{
    internal sealed class ProjectionDbContextFactory : DbContextFactory<DbContext, ProjectionDbType>, IProjectionDbContextFactory
    {
        public ProjectionDbContextFactory(IServiceProvider serviceProvider, IDbTypeFactory<ProjectionDbType> dbTypeFactory) : base(serviceProvider, dbTypeFactory)
        {
        }
    }
}
