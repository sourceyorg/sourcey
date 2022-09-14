using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zion.Extensions;
using Zion.Projections;
using Zion.Testing.Abstractions;

namespace Zion.AWS.DynamoDB.Projections.Tests
{
    public abstract class DynamoDBProjectionsSpecification<TProjection> : SpecificationWithConfiguration<DynamoDBProjectionsConfigurationFixture>
        where TProjection : class, IProjection
    {
        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.RegisterDynamoDBWriter<TProjection>(o =>
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
        protected DynamoDBProjectionsSpecification(DynamoDBProjectionsConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }
    }

    public abstract class DynamoDBProjectionsSpecification<TProjection, TResult> : SpecificationWithConfiguration<DynamoDBProjectionsConfigurationFixture, TResult>
        where TProjection : class, IProjection
    {
        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.RegisterDynamoDBWriter<TProjection>(o =>
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
        protected DynamoDBProjectionsSpecification(DynamoDBProjectionsConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }
    }
}
