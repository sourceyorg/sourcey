namespace Sourcey.Keys;

public readonly struct Actor : IEquatable<Actor>
{
    internal readonly string _value;

    internal Actor(string value)
    {
        _value = value;
    }

    public static Actor New()
    {
        var value = IdGenerator.New();

        return new Actor(value);
    }

    public static Actor From(string value)
        => new(value ?? IdGenerator.Unknown);

    public static readonly Actor Unknown = new(IdGenerator.Unknown);

    public bool Equals(Actor other) => _value == other._value;
    public override bool Equals(object? obj) => obj is Actor other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(Actor left, Actor right) => left.Equals(right);
    public static bool operator !=(Actor left, Actor right) => !left.Equals(right);
    public static implicit operator string(Actor id) => id._value;
    public static implicit operator Actor(string id) => From(id);
}
