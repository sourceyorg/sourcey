namespace Sourcey.Commands.Cache
{
    public interface ICommandTypeCache
    {
        bool TryGet(string name, out Type? type);
    }
}
