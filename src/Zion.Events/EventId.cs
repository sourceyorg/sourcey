using Zion.Core.Keys;

namespace Zion.Events
{
    public readonly struct EventId : IEquatable<EventId>
    {
        internal readonly string _value;

        internal EventId(string value)
        {
            _value = value;
        }

        public static EventId New()
        {
            var value = Base64UrlIdGenerator.New();

            return new EventId(value);
        }

        public static EventId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new EventId(value);
        }

        public bool Equals(EventId other) => _value == other._value;
        public override bool Equals(object? obj) => obj is EventId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(EventId left, EventId right) => left.Equals(right);
        public static bool operator !=(EventId left, EventId right) => !left.Equals(right);
        public static implicit operator string(EventId id) => id._value;
    }
}
