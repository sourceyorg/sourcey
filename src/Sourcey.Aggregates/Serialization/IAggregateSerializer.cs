namespace Sourcey.Aggregates.Serialization;

public interface IAggregateSerializer
{
    string Serialize<T>(T data);
}
