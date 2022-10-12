﻿using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Events.Entities;
using Zion.EntityFrameworkCore.Events.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Events.DbContexts
{
    public class EventStoreDbContextBase<TStoreDbContext> : DbContext, IEventStoreDbContext
        where TStoreDbContext : DbContext, IEventStoreDbContext
    {
        protected virtual string Schema => "log";

        public DbSet<Event> Events { get; set; }

        public EventStoreDbContextBase(DbContextOptions<TStoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EventEntityTypeConfiguration(Schema));

            base.OnModelCreating(builder);
        }
    }
}
