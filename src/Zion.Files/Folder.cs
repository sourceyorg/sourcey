using Zion.Core.Keys;

namespace Zion.Files
{
    public readonly struct Folder : IEquatable<Folder>
    {
        internal readonly string _value;

        internal Folder(string value)
        {
            _value = value;
        }

        public static Folder New()
        {
            var value = ZionIdGenerator.New();

            return new Folder(value);
        }

        public static Folder From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new Folder(value);
        }

        public static readonly Folder Unknown = new("Unknown");

        public bool Equals(Folder other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Folder other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(Folder left, Folder right) => left.Equals(right);
        public static bool operator !=(Folder left, Folder right) => !left.Equals(right);
        public static implicit operator string(Folder id) => id._value;
    }
}
