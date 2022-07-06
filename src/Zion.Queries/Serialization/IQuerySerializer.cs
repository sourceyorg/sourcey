namespace Zion.Queries.Serialization
{
    public interface IQuerySerializer
    {
        string Serialize<T>(T data);
    }
}
