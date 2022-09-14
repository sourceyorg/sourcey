using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Commands.Entities;
using Zion.EntityFrameworkCore.Extensions;

namespace Zion.EntityFrameworkCore.Commands.EntityTypeConfiguration
{
    internal class CommandEntityTypeConfiguration : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder.ToTable(name: nameof(Command), schema: "log");

            builder.HasKey(c => c.SequenceNo);
            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.Subject);
            builder.HasIndex(c => c.Correlation);
            builder.HasIndex(c => c.Name);
            builder.HasIndex(c => c.Actor);

            builder.Property(e => e.Id)
                .HasCommandIdValueConversion();
            builder.Property(e => e.Subject)
                .HasSubjectValueConversion();
            builder.Property(e => e.Correlation)
                .HasNullableCorrelationValueConversion();
            builder.Property(e => e.Actor)
                .HasActorValueConversion();
        }
    }
}
