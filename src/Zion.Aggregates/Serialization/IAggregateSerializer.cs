namespace Zion.Aggregates.Serialization
{
    public interface IAggregateSerializer
    {
        string Serialize<T>(T data);
    }
}
