namespace Sourcey.Events.Serialization
{
    public interface IEventDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
