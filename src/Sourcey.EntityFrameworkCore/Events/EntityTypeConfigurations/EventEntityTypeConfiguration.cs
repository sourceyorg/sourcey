using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sourcey.EntityFrameworkCore.Events.Entities;
using Sourcey.Extensions;

namespace Sourcey.EntityFrameworkCore.Events.EntityTypeConfigurations;

internal sealed class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
{
    private readonly string _schema;

    public EventEntityTypeConfiguration(string schema)
    {
        if(string.IsNullOrWhiteSpace(schema))
            throw new ArgumentNullException(nameof(schema));

        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable(name: nameof(Event), schema: _schema);

        builder.HasKey(c => c.SequenceNo);
        builder.HasIndex(c => c.Id);
        builder.HasIndex(c => c.StreamId);
        builder.HasIndex(c => c.Correlation);
        builder.HasIndex(c => c.Causation);
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Actor);
        builder.HasIndex(c => c.Version);

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
