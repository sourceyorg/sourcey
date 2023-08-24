using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sourcey.Commands;
using Sourcey.EntityFrameworkCore.Commands.ChangeTracking;
using Sourcey.EntityFrameworkCore.Commands.ValueConversion;

namespace Sourcey.EntityFrameworkCore.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<CommandId> HasCommandIdValueConversion(this PropertyBuilder<CommandId> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new CommandIdValueConverter())
                   .Metadata
                   .SetValueComparer(new CommandIdValueComparer());

            return builder;
        }

        public static PropertyBuilder<CommandId?> HasNullableCommandIdValueConversion(this PropertyBuilder<CommandId?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableCommandIdValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableCommandIdValueComparer());

            return builder;
        }
    }
}
