namespace Sourcey.Core.Keys
{
    public readonly struct Causation : IEquatable<Causation>
    {
        internal readonly string _value;

        internal Causation(string value)
        {
            _value = value;
        }

        public static Causation New()
        {
            var value = IdGenerator.New();

            return new Causation(value);
        }

        public static Causation From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Causation(value);
        }

        public bool Equals(Causation other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Causation other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Causation left, Causation right) => left.Equals(right);
        public static bool operator !=(Causation left, Causation right) => !left.Equals(right);
        public static implicit operator string(Causation id) => id._value;
    }
}
