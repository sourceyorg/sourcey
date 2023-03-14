using Zion.Core.Keys;

namespace Zion.AWS.SQS
{
    public readonly struct SQSQueue : IEquatable<SQSQueue>
    {
        internal readonly string _value;

        internal SQSQueue(string value)
        {
            _value = value;
        }

        public static SQSQueue New()
        {
            var value = ZionIdGenerator.New();

            return new SQSQueue(value);
        }

        public static SQSQueue From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new SQSQueue(value);
        }

        public bool Equals(SQSQueue other) => _value == other._value;
        public override bool Equals(object? obj) => obj is SQSQueue other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(SQSQueue left, SQSQueue right) => left.Equals(right);
        public static bool operator !=(SQSQueue left, SQSQueue right) => !left.Equals(right);
        public static implicit operator string(SQSQueue id) => id._value;
    }
}
