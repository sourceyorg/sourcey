namespace Zion.Core.Keys
{
    public readonly struct Subject : IEquatable<Subject>
    {
        internal readonly string _value;

        internal Subject(string value)
        {
            _value = value;
        }

        public static Subject New()
        {
            var value = ZionIdGenerator.New();

            return new Subject(value);
        }

        public static Subject From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Subject(value);
        }

        public bool Equals(Subject other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Subject other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Subject left, Subject right) => left.Equals(right);
        public static bool operator !=(Subject left, Subject right) => !left.Equals(right);
        public static implicit operator string(Subject id) => id._value;
    }
}
