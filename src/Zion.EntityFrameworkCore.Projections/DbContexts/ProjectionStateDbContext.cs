using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Projections.DbContexts
{
    public class ProjectionStateDbContext : DbContext
    {
        public DbSet<ProjectionState> ProjectionStates { get; set; }

        protected ProjectionStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectionStateEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
