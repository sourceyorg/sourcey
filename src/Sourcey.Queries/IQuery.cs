using Sourcey.Core.Keys;

namespace Sourcey.Queries
{
    public interface IQuery<out TQueryResult>
    {
        QueryId Id { get; }
        DateTimeOffset Timestamp { get; }
        Correlation? Correlation { get; }
        Actor Actor { get; }
    }
}
