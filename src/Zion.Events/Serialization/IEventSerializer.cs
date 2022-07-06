namespace Zion.Events.Serialization
{
    public interface IEventSerializer
    {
        string Serialize<T>(T data);
    }
}
