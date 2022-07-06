using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Commands.Entities;
using Zion.EntityFrameworkCore.Commands.EntityTypeConfiguration;

namespace Zion.EntityFrameworkCore.Commands.DbContexts
{
    public sealed class CommandStoreDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }

        public CommandStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CommandEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
