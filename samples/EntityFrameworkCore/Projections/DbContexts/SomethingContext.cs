using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace EntityFrameworkCore.Projections.DbContexts;

public abstract class SomethingContext(DbContextOptions options) : ProjectionStateDbContext(options)
{
    protected override string Schema => "Sample";

    public DbSet<Something> Somethings { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Something>(entity =>
        {
            entity.ToTable("Something");
            entity.HasKey(e => e.Subject);
            entity.HasIndex(e => e.Value);
        });

        base.OnModelCreating(modelBuilder);
    }
}
