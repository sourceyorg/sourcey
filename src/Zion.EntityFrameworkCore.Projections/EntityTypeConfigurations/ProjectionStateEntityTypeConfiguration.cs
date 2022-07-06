using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Projections.Entities;

namespace Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations
{
    internal sealed class ProjectionStateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectionState>
    {
        public void Configure(EntityTypeBuilder<ProjectionState> builder)
        {
            builder.ToTable(name: nameof(ProjectionState));
            builder.HasKey(c => c.Key);
        }
    }
}
