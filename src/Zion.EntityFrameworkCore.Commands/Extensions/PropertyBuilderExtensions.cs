using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.Commands;
using Zion.EntityFrameworkCore.Commands.ChangeTracking;
using Zion.EntityFrameworkCore.Commands.ValueConversion;

namespace Zion.EntityFrameworkCore.Extensions
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
