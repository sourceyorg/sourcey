using Zion.Core.Keys;

namespace Zion.Events.Streams
{
    public readonly struct StreamId : IEquatable<StreamId>
    {
        internal readonly string _value;

        internal StreamId(string value)
        {
            _value = value;
        }

        public static StreamId New()
        {
            var value = ZionIdGenerator.New();

            return new StreamId(value);
        }

        public static StreamId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new StreamId(value);
        }

        public bool Equals(StreamId other) => _value == other._value;
        public override bool Equals(object? obj) => obj is StreamId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(StreamId left, StreamId right) => left.Equals(right);
        public static bool operator !=(StreamId left, StreamId right) => !left.Equals(right);
        public static implicit operator string(StreamId id) => id._value;
    }
}
