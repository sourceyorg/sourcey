using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections
{
    public interface IProjectionTableInitializer
    {
        Task CreateTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection;
        Task DeleteTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection;
        Task ResetTableAsync<TProjection>(string? tableOverride = null, CancellationToken cancellationToken = default)
            where TProjection : class, IProjection;
    }
}
