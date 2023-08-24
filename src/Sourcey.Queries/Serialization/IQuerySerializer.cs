namespace Sourcey.Queries.Serialization
{
    public interface IQuerySerializer
    {
        string Serialize<T>(T data);
    }
}
