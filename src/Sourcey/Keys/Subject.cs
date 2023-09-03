namespace Sourcey.Keys;

public readonly struct Subject : IEquatable<Subject>
{
    internal readonly string _value;

    internal Subject(string value)
    {
        _value = value;
    }

    public static Subject New()
    {
        var value = IdGenerator.New();

        return new Subject(value);
    }

    public static Subject From(string value)
        => new(value ?? IdGenerator.Unknown);

    public bool Equals(Subject other) => _value == other._value;
    public override bool Equals(object? obj) => obj is Subject other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(Subject left, Subject right) => left.Equals(right);
    public static bool operator !=(Subject left, Subject right) => !left.Equals(right);
    public static implicit operator string(Subject id) => id._value;
    public static implicit operator Subject(string id) => From(id);
}
