using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Extensions;
using Zion.EntityFrameworkCore.Queries.Entities;
using Zion.EntityFrameworkCore.Queries.Extensions;

namespace Zion.EntityFrameworkCore.Queries.EntityTypeConfigurations
{
    internal class QueryEntityTypeConfiguration : IEntityTypeConfiguration<Query>
    {
        public void Configure(EntityTypeBuilder<Query> builder)
        {
            builder.ToTable(name: nameof(Query), schema: "log");

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
