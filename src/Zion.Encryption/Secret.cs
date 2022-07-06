namespace Zion.Encryption
{
    public readonly struct Secret : IEquatable<Secret>
    {
        internal readonly byte[] _value;

        internal Secret(byte[] value)
        {
            _value = value;
        }

        public static Secret From(byte[] value)
        {
            return new Secret(value);
        }

        public bool Equals(Secret other) => _value == other._value;
        public override bool Equals(object? obj) => obj is Secret other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(Secret left, Secret right) => left.Equals(right);
        public static bool operator !=(Secret left, Secret right) => !left.Equals(right);
        public static implicit operator byte[](Secret id) => id._value;
    }
}
