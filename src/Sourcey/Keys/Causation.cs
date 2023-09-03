namespace Sourcey.Keys;

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
        => new(value ?? IdGenerator.Unknown);

    public static readonly Causation Unknown = new(IdGenerator.Unknown);

    public bool Equals(Causation other) => _value == other._value;
    public override bool Equals(object? obj) => obj is Causation other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(Causation left, Causation right) => left.Equals(right);
    public static bool operator !=(Causation left, Causation right) => !left.Equals(right);
    public static implicit operator string(Causation id) => id._value;
    public static implicit operator Causation(string id) => From(id);
}
