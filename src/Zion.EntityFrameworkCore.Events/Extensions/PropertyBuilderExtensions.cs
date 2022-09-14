using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Events.ChangeTracking;
using Zion.EntityFrameworkCore.Events.ValueConversion;
using Zion.Events;
using Zion.Events.Streams;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<EventId> HasEventIdValueConversion(this PropertyBuilder<EventId> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new EventIdValueConverter())
                   .Metadata
                   .SetValueComparer(new EventIdValueComparer());

            return builder;
        }

        public static PropertyBuilder<EventId?> HasNullableEventIdValueConversion(this PropertyBuilder<EventId?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableEventIdValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableEventIdValueComparer());

            return builder;
        }

        public static PropertyBuilder<StreamId> HasStreamIdValueConversion(this PropertyBuilder<StreamId> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new StreamIdValueConverter())
                   .Metadata
                   .SetValueComparer(new StreamIdValueComparer());

            return builder;
        }

        public static PropertyBuilder<StreamId?> HasNullableStreamIdValueConversion(this PropertyBuilder<StreamId?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableStreamIdValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableStreamIdValueComparer());

            return builder;
        }
    }
}
