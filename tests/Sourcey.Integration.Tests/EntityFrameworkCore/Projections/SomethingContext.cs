using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;
using Sourcey.Testing.Integration.Stubs.Projections;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections;

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
