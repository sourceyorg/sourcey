using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations;

namespace Zion.EntityFrameworkCore.Projections.DbContexts
{
    public abstract class ProjectionDbContext : DbContext
    {
        public DbSet<ProjectionState> ProjectionStates { get; set; }

        protected ProjectionDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectionStateEntityTypeConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
