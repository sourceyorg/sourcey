using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Commands.Entities;
using Sourcey.EntityFrameworkCore.Commands.EntityTypeConfiguration;

namespace Sourcey.EntityFrameworkCore.Commands.DbContexts
{
    public abstract class CommandStoreDbContext : DbContext
    {
        protected virtual string Schema => "log";

        public DbSet<Command> Commands { get; set; }

        protected CommandStoreDbContext(DbContextOptions options) 
            : base(options) { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CommandEntityTypeConfiguration(Schema));

            base.OnModelCreating(builder);
        }
    }
}
