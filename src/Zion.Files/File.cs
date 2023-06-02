using Zion.Core.Keys;

namespace Zion.Files
{
    public readonly struct File : IEquatable<File>
    {
        internal readonly string _value;

        internal File(string value)
        {
            _value = value;
        }

        public static File New()
        {
            var value = ZionIdGenerator.New();

            return new File(value);
        }

        public static File From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new File(value);
        }

        public static readonly File Unknown = new("Unknown");

        public bool Equals(File other) => _value == other._value;
        public override bool Equals(object? obj) => obj is File other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(File left, File right) => left.Equals(right);
        public static bool operator !=(File left, File right) => !left.Equals(right);
        public static implicit operator string(File id) => id._value;
    }
}
