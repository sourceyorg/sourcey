namespace Sourcey.Keys;

public readonly struct EventId : IEquatable<EventId>
{
    internal readonly string _value;

    internal EventId(string value)
    {
        _value = value;
    }

    public static EventId New()
    {
        var value = IdGenerator.New();

        return new EventId(value);
    }

    public static EventId From(string value)
        => new(value ?? IdGenerator.Unknown);

    public static readonly EventId Unknown = new(IdGenerator.Unknown);

    public bool Equals(EventId other) => _value == other._value;
    public override bool Equals(object? obj) => obj is EventId other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(EventId left, EventId right) => left.Equals(right);
    public static bool operator !=(EventId left, EventId right) => !left.Equals(right);
    public static implicit operator string(EventId id) => id._value;
    public static implicit operator EventId(string id) => From(id);
}
