using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.EntityFrameworkCore.Queries.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Queries.DbContexts
{
    public abstract class QueryStoreDbContext : DbContext
    {
        protected virtual string Schema => "log";

        public DbSet<Query> Queries { get; set; }

        protected QueryStoreDbContext(DbContextOptions options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new QueryEntityTypeConfiguration(Schema));

            base.OnModelCreating(builder);
        }
    }
}
