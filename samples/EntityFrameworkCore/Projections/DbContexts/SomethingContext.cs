using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace EntityFrameworkCore.Projections.DbContexts;

public class SomethingContext : ProjectionStateDbContext
{
    protected override string Schema => "Sample";

    DbSet<Something> Somethings { get; set;}

    public SomethingContext(DbContextOptions<SomethingContext> options) : base(options)
    {
    }

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