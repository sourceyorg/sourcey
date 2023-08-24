namespace Sourcey.Commands.Serialization
{
    public interface ICommandSerializer
    {
        string Serialize<T>(T data);
    }
}
