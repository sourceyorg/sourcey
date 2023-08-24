namespace Sourcey.Events.Serialization
{
    public interface IEventSerializer
    {
        string Serialize<T>(T data);
    }
}
