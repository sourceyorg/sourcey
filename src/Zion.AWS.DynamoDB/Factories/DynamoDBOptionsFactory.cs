using Zion.Core.Extensions;
using Zion.Projections;

namespace Zion.AWS.DynamoDB.Projections.Factories
{
    internal sealed class DynamoDBOptionsFactory : IDynamoDBOptionsFactory
    {
        private readonly IDictionary<string, DynamoDBOptions> _options;

        public DynamoDBOptionsFactory(IEnumerable<DynamoDBConfiguration> projectionDbTypes)
        {
            _options = projectionDbTypes?
                .DistinctBy(pdbt => pdbt.ProjectionType.FriendlyName())
                ?.ToDictionary(pdbt => pdbt.ProjectionType.FriendlyName(), pdbt => pdbt.Options)
                ?? new Dictionary<string, DynamoDBOptions>();
        }

        public DynamoDBOptions Create<TService>()
            where TService : class
        {
            var serviceName = typeof(TService).FriendlyName();

            if (!_options.TryGetValue(serviceName, out var options) || options is null)
            {
                throw new InvalidOperationException($"No db context registered against: {serviceName}");
            }

            return options;
        }
    }
}
