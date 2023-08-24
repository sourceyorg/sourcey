using Sourcey.Core.Keys;

namespace Sourcey.Commands
{
    public readonly struct CommandId : IEquatable<CommandId>
    {
        internal readonly string _value;

        internal CommandId(string value)
        {
            _value = value;
        }

        public static CommandId New()
        {
            var value = IdGenerator.New();

            return new CommandId(value);
        }

        public static CommandId From(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));

            return new CommandId(value);
        }

        public bool Equals(CommandId other) => _value == other._value;
        public override bool Equals(object? obj) => obj is CommandId other && Equals(other);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;

        public static bool operator ==(CommandId left, CommandId right) => left.Equals(right);
        public static bool operator !=(CommandId left, CommandId right) => !left.Equals(right);
        public static implicit operator string(CommandId id) => id._value;
    }
}
