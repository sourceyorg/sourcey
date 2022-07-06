using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zion.Queries;

namespace Zion.EntityFrameworkCore.Queries.ValueConversion
{
    internal sealed class QueryIdValueConverter : ValueConverter<QueryId, string>
    {
        public QueryIdValueConverter(ConverterMappingHints mappingHints = default)
            : base((m) => ConvertTo(m), (json) => ConvertFrom(json), mappingHints)
        {
        }

        private static string ConvertTo(QueryId queryId)
            => (string)queryId;

        private static QueryId ConvertFrom(string queryId)
            => QueryId.From(queryId);
    }
}
