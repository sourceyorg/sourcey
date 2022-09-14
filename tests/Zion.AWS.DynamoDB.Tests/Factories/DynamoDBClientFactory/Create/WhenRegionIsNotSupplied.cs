using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Abstractions;
using Zion.Extensions;
using Zion.Testing.Attributes;

namespace Zion.AWS.DynamoDB.Tests.Factories.DynamoDBClientFactory.Create
{
    public class WhenRegionIsNotSupplied : DynamoDBClientFactorySpecification<AmazonDynamoDBClient>
    {
        public WhenRegionIsNotSupplied(DynamoDBConfigurationFixture configurationFixture, ITestOutputHelper testOutputHelper) : base(configurationFixture, testOutputHelper)
        {
        }

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.AddDynamoDBOptions<WhenRegionIsSupplied>(o =>
            {
                o.Key = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Key");
                o.Secret = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Secret");
                o.ClientConfig = new()
                {
                    ServiceURL = _configurationFixture.Configuration.GetValue<string>("AWS:DynamoDB:Host")
                };
            });

            return base.BuildServices(services);
        }


        protected override Task<AmazonDynamoDBClient> Given()
        {
            return Task.FromResult(Factory.Create<WhenRegionIsSupplied>());
        }

        protected override Task When() => Task.CompletedTask;

        [Then]
        public void ThenClientShouldNotBeNull()
        {
            Result.ShouldNotBeNull();
        }

        [Then]
        public async Task ThenClientShouldBeConnected()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(3000);

            var result = await Result?.ListTablesAsync(cts.Token);
            result.ShouldNotBeNull();
        }
    }
}
