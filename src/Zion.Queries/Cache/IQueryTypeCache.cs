namespace Zion.Queries.Cache
{
    public interface IQueryTypeCache
    {
        bool TryGet(string name, out Type? type);
    }
}
