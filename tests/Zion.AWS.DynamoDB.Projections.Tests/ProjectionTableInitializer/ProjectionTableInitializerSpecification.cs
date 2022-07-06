using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zion.AWS.DynamoDB.Projections.Factories;

namespace Zion.AWS.DynamoDB.Projections.Tests.ProjectionTableInitializer
{
    public abstract class ProjectionTableInitializerSpecification : DynamoDBProjectionsSpecification<TestProjection>
    {
        protected readonly IProjectionTableInitializer _projectionTableInitializer;
        protected readonly IDynamoDBClientFactory _dynamoDBClientFactory;

        protected ProjectionTableInitializerSpecification(DynamoDBProjectionsConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper) : base(configurationFixture, testOutputHelper)
        {
            _projectionTableInitializer = _serviceProvider.GetRequiredService<IProjectionTableInitializer>();
            _dynamoDBClientFactory = _serviceProvider.GetRequiredService<IDynamoDBClientFactory>();
        }
    }
}
