using Shouldly;
using Xunit.Abstractions;
using Zion.Core.Keys;
using Zion.Testing.Attributes;

namespace Zion.AWS.DynamoDB.Projections.Tests.ProjectionTableInitializer.CreateTableAsync
{
    public class WhenTableDoesNotAlreadyExist : ProjectionTableInitializerSpecification
    {
        private Subject _tableName;

        public WhenTableDoesNotAlreadyExist(DynamoDBProjectionsConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper) : base(configurationFixture, testOutputHelper)
        {
        }

        protected override async Task Given()
        {
            await _projectionTableInitializer.CreateTableAsync<TestProjection>(_tableName);
        }

        protected override Task When() 
        {
            _tableName = Subject.New();
            return Task.CompletedTask;
        }

        [Then]
        public async Task ThenTableShouldBeSuccessfullyCreated()
        {
            using var client = _dynamoDBClientFactory.Create<TestProjection>();
            var result = await client.DescribeTableAsync(_tableName);

            result.HttpStatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        }

        public override async Task DisposeAsync()
        {
            await _projectionTableInitializer.DeleteTableAsync<TestProjection>(_tableName);

            await base.DisposeAsync();
        }
    }
}
