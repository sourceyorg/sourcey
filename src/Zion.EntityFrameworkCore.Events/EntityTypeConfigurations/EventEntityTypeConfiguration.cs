using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Events.Entities;
using Zion.EntityFrameworkCore.Events.Extensions;
using Zion.EntityFrameworkCore.Extensions;

namespace Zion.EntityFrameworkCore.Events.EntityTypeConfigurations
{
    internal sealed class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable(name: nameof(Event), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.StreamId);
            builder.HasIndex(c => c.Correlation);
            builder.HasIndex(c => c.Causation);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Actor);

            builder.Property(e => e.Id)
                .HasEventIdValueConversion();
            builder.Property(e => e.StreamId)
                .HasStreamIdValueConversion();
            builder.Property(e => e.Causation)
                .HasNullableCausationValueConversion();
            builder.Property(e => e.Correlation)
                .HasNullableCorrelationValueConversion();
            builder.Property(e => e.Actor)
                .HasActorValueConversion();
        }
    }
}
