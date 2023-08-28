using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sourcey.EntityFrameworkCore.Events.ChangeTracking;
using Sourcey.EntityFrameworkCore.Events.ValueConversion;
using Sourcey.Events;
using Sourcey.Events.Streams;

namespace Sourcey.Extensions;

public static partial class PropertyBuilderExtensions
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
