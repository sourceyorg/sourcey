namespace Sourcey.Projections.Serialization;

public interface IProjectionSerializer
{
    string Serialize<T>(T data);
}
