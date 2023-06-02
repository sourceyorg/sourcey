using Zion.Core.Keys;

namespace Zion.Files
{
    public readonly struct Root : IEquatable<Root>
    {
        internal readonly string _value;

        internal Root(string value)
        {
            _value = value;
        }

        public static Root New()
        {
            var value = ZionIdGenerator.New();

            return new Root(value);
        }

        public static Root From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Root(value);
        }

        public static readonly Root Unknown = new("Unknown");

        public bool Equals(Root other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Root other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Root left, Root right) => left.Equals(right);
        public static bool operator !=(Root left, Root right) => !left.Equals(right);
        public static implicit operator string(Root id) => id._value;
    }
}
