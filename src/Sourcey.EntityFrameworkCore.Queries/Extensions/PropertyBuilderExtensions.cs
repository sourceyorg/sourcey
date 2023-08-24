using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sourcey.EntityFrameworkCore.Queries.ChangeTracking;
using Sourcey.EntityFrameworkCore.Queries.ValueConversion;
using Sourcey.Queries;

namespace Sourcey.EntityFrameworkCore.Extensions
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
