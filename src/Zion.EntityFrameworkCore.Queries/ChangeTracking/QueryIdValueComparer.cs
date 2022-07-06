using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Queries;

namespace Zion.EntityFrameworkCore.Queries.ChangeTracking
{
    internal sealed class QueryIdValueComparer : ValueComparer<QueryId>
    {
        public QueryIdValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(QueryId left, QueryId right)
            => left.Equals(right);
        private static int HashCode(QueryId queryId)
            => queryId.GetHashCode();
        private static QueryId CreateSnapshot(QueryId queryId)
            => ConvertFrom(ConvertTo(queryId));
        private static string ConvertTo(QueryId queryId)
            => (string)queryId;
        private static QueryId ConvertFrom(string queryId)
            => QueryId.From(queryId);
    }
}
