using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Queries;

namespace Zion.EntityFrameworkCore.Queries.ValueConversion
{
    internal sealed class NullableQueryIdValueConverter : ValueConverter<QueryId?, string>
    {
        public NullableQueryIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(QueryId? queryId)
            => queryId.HasValue ? (string)queryId.Value : string.Empty;

        private static QueryId? ConvertFrom(string queryId)
            => string.IsNullOrWhiteSpace(queryId) ? null : QueryId.From(queryId);
    }
}
