namespace Zion.Core.Keys
{
    public readonly struct Actor : IEquatable<Actor>
    {
        internal readonly string _value;

        internal Actor(string value)
        {
            _value = value;
        }

        public static Actor New()
        {
            var value = ZionIdGenerator.New();

            return new Actor(value);
        }

        public static Actor From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Actor(value);
        }

        public static readonly Actor Unknown = new("Unknown");

        public bool Equals(Actor other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Actor other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Actor left, Actor right) => left.Equals(right);
        public static bool operator !=(Actor left, Actor right) => !left.Equals(right);
        public static implicit operator string(Actor id) => id._value;
    }
}
