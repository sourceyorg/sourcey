using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Projections.Entities;

namespace Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations
{
    internal sealed class ProjectionStateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectionState>
    {
        private readonly string _schema;

        public ProjectionStateEntityTypeConfiguration(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentNullException(nameof(schema));

            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<ProjectionState> builder)
        {
            builder.ToTable(name: nameof(ProjectionState), schema: _schema);
            builder.HasKey(c => c.Key);
        }
    }
}
