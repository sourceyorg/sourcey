using Zion.Core.Keys;

namespace Zion.Queries
{
    public interface IQuery<out TQueryResult>
    {
        QueryId Id { get; }
        DateTimeOffset Timestamp { get; }
        Correlation? Correlation { get; }
        Actor Actor { get; }
    }
}
