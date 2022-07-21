using Microsoft.EntityFrameworkCore;

namespace Zion.EntityFrameworkCore.Projections.DbContexts
{
    public class ProjectionStateDbContext : ProjectionStateDbContextBase<ProjectionStateDbContext>
    {
        protected ProjectionStateDbContext(DbContextOptions<ProjectionStateDbContext> options) : base(options)
        {
        }
    }
}
