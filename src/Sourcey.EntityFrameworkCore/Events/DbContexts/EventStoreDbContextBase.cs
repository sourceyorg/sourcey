using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.Entities;
using Sourcey.EntityFrameworkCore.Events.EntityTypeConfigurations;
using Sourcey.Extensions;

namespace Sourcey.EntityFrameworkCore.Events.DbContexts;

public class EventStoreDbContextBase<TStoreDbContext> : DbContext, IEventStoreDbContext
    where TStoreDbContext : DbContext, IEventStoreDbContext
{
    protected virtual string Schema => "log";

    public DbSet<Event> Events { get; set; }

    public EventStoreDbContextBase(DbContextOptions<TStoreDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyEventConfiguration(Schema);
        base.OnModelCreating(builder);
    }
}
