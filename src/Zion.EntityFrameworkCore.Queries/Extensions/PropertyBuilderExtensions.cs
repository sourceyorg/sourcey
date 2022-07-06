using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zion.EntityFrameworkCore.Queries.ChangeTracking;
using Zion.EntityFrameworkCore.Queries.ValueConversion;
using Zion.Queries;

namespace Zion.EntityFrameworkCore.Queries.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<QueryId> HasQueryIdValueConversion(this PropertyBuilder<QueryId> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new QueryIdValueConverter())
                   .Metadata
                   .SetValueComparer(new QueryIdValueComparer());

            return builder;
        }

        public static PropertyBuilder<QueryId?> HasNullableQueryIdValueConversion(this PropertyBuilder<QueryId?> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasConversion(new NullableQueryIdValueConverter())
                   .Metadata
                   .SetValueComparer(new NullableQueryIdValueComparer());

            return builder;
        }
    }
}
