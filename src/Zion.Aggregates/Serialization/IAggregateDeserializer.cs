namespace Zion.Aggregates.Serialization
{
    public interface IAggregateDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
