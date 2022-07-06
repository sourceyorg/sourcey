namespace Zion.Core.Keys
{
    public readonly struct Correlation : IEquatable<Correlation>
    {
        internal readonly string _value;

        internal Correlation(string value)
        {
            _value = value;
        }

        public static Correlation New()
        {
            var value = Base64UrlIdGenerator.New();

            return new Correlation(value);
        }

        public static Correlation From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Correlation(value);
        }

        public bool Equals(Correlation other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Correlation other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Correlation left, Correlation right) => left.Equals(right);
        public static bool operator !=(Correlation left, Correlation right) => !left.Equals(right);
        public static implicit operator string(Correlation id) => id._value;
    }
}
