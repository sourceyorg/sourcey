namespace Sourcey.Keys;

public readonly struct StreamId : IEquatable<StreamId>
{
    internal readonly string _value;

    internal StreamId(string value)
    {
        _value = value;
    }

    public static StreamId New()
    {
        var value = IdGenerator.New();

        return new StreamId(value);
    }

    public static StreamId From(string value)
        => new(value ?? IdGenerator.Unknown);

    public static readonly StreamId Unknown = new(IdGenerator.Unknown);

    public bool Equals(StreamId other) => _value == other._value;
    public override bool Equals(object? obj) => obj is StreamId other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(StreamId left, StreamId right) => left.Equals(right);
    public static bool operator !=(StreamId left, StreamId right) => !left.Equals(right);
    public static implicit operator string(StreamId id) => id._value;
    public static implicit operator StreamId(string id) => From(id);
}
