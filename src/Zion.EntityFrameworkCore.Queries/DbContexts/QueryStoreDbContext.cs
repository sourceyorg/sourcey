using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.EntityFrameworkCore.Queries.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Queries.DbContexts
{
    public sealed class QueryStoreDbContext : DbContext
    {
        public DbSet<Query> Queries { get; set; }

        public QueryStoreDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new QueryEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
