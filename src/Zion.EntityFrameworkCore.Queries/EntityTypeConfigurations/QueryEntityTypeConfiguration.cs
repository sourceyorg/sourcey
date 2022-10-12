using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Extensions;
using Zion.EntityFrameworkCore.Queries.Entities;

namespace Zion.EntityFrameworkCore.Queries.EntityTypeConfigurations
{
    internal sealed class QueryEntityTypeConfiguration : IEntityTypeConfiguration<Query>
    {
        private readonly string _schema;

        public QueryEntityTypeConfiguration(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentNullException(nameof(schema));

            _schema = schema;
        }

        public void Configure(EntityTypeBuilder<Query> builder)
        {
            builder.ToTable(name: nameof(Query), schema: _schema);

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.Correlation);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Actor);

            builder.Property(e => e.Id)
                .HasQueryIdValueConversion();
            builder.Property(e => e.Correlation)
                .HasNullableCorrelationValueConversion();
            builder.Property(e => e.Actor)
                .HasActorValueConversion();
        }
    }
}
