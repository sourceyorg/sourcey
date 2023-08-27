using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.Entities;
using Sourcey.EntityFrameworkCore.Projections.EntityTypeConfigurations;

namespace Sourcey.EntityFrameworkCore.Projections.DbContexts;

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
