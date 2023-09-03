namespace Sourcey.Keys;

public readonly struct Correlation : IEquatable<Correlation>
{
    internal readonly string _value;

    internal Correlation(string value)
    {
        _value = value;
    }

    public static Correlation New()
    {
        var value = IdGenerator.New();

        return new Correlation(value);
    }

    public static Correlation From(string value)
        => new(value ?? IdGenerator.Unknown);

    public static readonly Correlation Unknown = new(IdGenerator.Unknown);

    public bool Equals(Correlation other) => _value == other._value;
    public override bool Equals(object? obj) => obj is Correlation other && Equals(other);
    public override int GetHashCode() => _value.GetHashCode();
    public override string ToString() => _value;

    public static bool operator ==(Correlation left, Correlation right) => left.Equals(right);
    public static bool operator !=(Correlation left, Correlation right) => !left.Equals(right);
    public static implicit operator string(Correlation id) => id._value;
    public static implicit operator Correlation(string id) => From(id);
}
