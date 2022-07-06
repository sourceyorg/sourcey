using Zion.Core.Keys;

namespace Zion.Queries
{
    public readonly struct QueryId : IEquatable<QueryId>
    {
        internal readonly string _value;

        internal QueryId(string value)
        {
            _value = value;
        }

        public static QueryId New()
        {
            var value = Base64UrlIdGenerator.New();

            return new QueryId(value);
        }

        public static QueryId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new QueryId(value);
        }

        public bool Equals(QueryId other) => _value == other._value;
        public override bool Equals(object? obj) => obj is QueryId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(QueryId left, QueryId right) => left.Equals(right);
        public static bool operator !=(QueryId left, QueryId right) => !left.Equals(right);
        public static implicit operator string(QueryId id) => id._value;
    }
}
