using Zion.Core.Extensions;

namespace Zion.EntityFrameworkCore.Projections.Factories.DbContexts
{
    internal sealed class DbTypeFactory<TDbType> : IDbTypeFactory<TDbType>
        where TDbType : DbType
    {
        private readonly IDictionary<string, TDbType> _dbTypes;

        public DbTypeFactory(IEnumerable<TDbType> dbTypes)
        {
            _dbTypes = dbTypes?
                .DistinctBy(pdbt => pdbt.ProjectionType.FriendlyName())
                ?.ToDictionary(pdbt => pdbt.ProjectionType.FriendlyName(), pdbt => pdbt)
                ?? new Dictionary<string, TDbType>();

        }

        TDbType IDbTypeFactory<TDbType>.Create<TProjection>()
        {
            var projectionName = typeof(TProjection).FriendlyFullName();

            if (!_dbTypes.TryGetValue(projectionName, out var types) || types is null)
            {
                throw new InvalidOperationException($"No db registered against projection: {projectionName}");
            }

            return types;
        }
    }
}
