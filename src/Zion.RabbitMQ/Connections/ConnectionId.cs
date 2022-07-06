using Zion.Core.Keys;

namespace Zion.RabbitMQ.Connections
{
    public readonly struct ConnectionId : IEquatable<ConnectionId>
    {
        internal readonly string _value;

        internal ConnectionId(string value)
        {
            _value = value;
        }

        public static ConnectionId New()
        {
            var value = Base64UrlIdGenerator.New();

            return new ConnectionId(value);
        }

        public static ConnectionId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new ConnectionId(value);
        }

        public bool Equals(ConnectionId other) => _value == other._value;
        public override bool Equals(object? obj) => obj is ConnectionId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(ConnectionId left, ConnectionId right) => left.Equals(right);
        public static bool operator !=(ConnectionId left, ConnectionId right) => !left.Equals(right);
        public static implicit operator string(ConnectionId id) => id._value;
    }
}
