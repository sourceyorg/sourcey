using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Queries;

namespace Sourcey.EntityFrameworkCore.Queries.ChangeTracking
{
    internal sealed class NullableQueryIdValueComparer : ValueComparer<QueryId?>
    {
        public NullableQueryIdValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(QueryId? left, QueryId? right)
        {
            if (left == null || right == null)
                return false;

            return left.Value.Equals(right.Value);
        }

        private static int HashCode(QueryId? queryId)
        {
            if (queryId == null)
                return 0;

            if (queryId is IEquatable<QueryId>)
                return queryId.GetHashCode();

            return ConvertTo(queryId)?.GetHashCode() ?? 0;
        }

        private static QueryId? CreateSnapshot(QueryId? queryId)
        {
            return ConvertFrom(ConvertTo(queryId));
        }

        private static string? ConvertTo(QueryId? queryId)
        {
            if (queryId == null)
                return null;

            return (string)queryId;
        }

        private static QueryId? ConvertFrom(string? queryId)
        {
            if (string.IsNullOrWhiteSpace(queryId))
                return null;

            return QueryId.From(queryId);
        }
    }
}
