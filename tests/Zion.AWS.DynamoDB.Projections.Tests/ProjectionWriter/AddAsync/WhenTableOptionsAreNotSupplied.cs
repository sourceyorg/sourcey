using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Zion.Core.Keys;
using Zion.Projections;
using Zion.Testing.Attributes;

namespace Zion.AWS.DynamoDB.Projections.Tests.ProjectionWriter.AddAsync
{
    public class WhenTableOptionsAreNotSupplied : ProjectionWriterSpecification
    {
        private readonly Subject _subject = Subject.New();

        public WhenTableOptionsAreNotSupplied(DynamoDBProjectionsConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }

        protected override Task<TestProjection> Given()
            => _serviceProvider.GetRequiredService<IProjectionReader<TestProjection>>().RetrieveAsync(_subject);

        protected override async Task When()
        {
            await _projectionTableInitializer.CreateTableAsync<TestProjection>(_tableName);
            await _projectionWriter.AddAsync(_subject, () => new TestProjection { Something = _subject });
        }

        [Then]
        public void ThenProjectionShouldBeCreated()
        {
            Result.ShouldNotBeNull();
        }

        [Then]
        public void ThenProjectionShouldHaveExpectedKey()
        {
            Result?.Something.ShouldBe(_subject);
        }

        public override async Task DisposeAsync()
        {
            await _projectionTableInitializer.DeleteTableAsync<TestProjection>(_tableName);
            await base.DisposeAsync();
        }
    }
}
