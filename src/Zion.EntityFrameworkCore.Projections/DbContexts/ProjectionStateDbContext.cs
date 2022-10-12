using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Projections.DbContexts
{
    public abstract class ProjectionStateDbContext : DbContext
    {
        protected virtual string Schema => "log";

        public DbSet<ProjectionState> ProjectionStates { get; set; }

        protected ProjectionStateDbContext(DbContextOptions options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectionStateEntityTypeConfiguration(Schema));

            base.OnModelCreating(builder);
        }
    }
}
