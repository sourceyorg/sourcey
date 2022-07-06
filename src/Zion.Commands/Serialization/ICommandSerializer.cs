namespace Zion.Commands.Serialization
{
    public interface ICommandSerializer
    {
        string Serialize<T>(T data);
    }
}
