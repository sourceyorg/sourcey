namespace Zion.Projections.Serialization
{
    public interface IProjectionDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
