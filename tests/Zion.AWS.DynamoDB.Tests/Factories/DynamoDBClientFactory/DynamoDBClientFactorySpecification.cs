using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zion.AWS.DynamoDB.Projections.Factories;
using Zion.Extensions;
using Zion.Testing.Abstractions;

namespace Zion.AWS.DynamoDB.Tests.Factories.DynamoDBClientFactory
{
    public abstract class DynamoDBClientFactorySpecification : SpecificationWithConfiguration<DynamoDBConfigurationFixture>
    {
        protected IDynamoDBClientFactory Factory => _serviceProvider.GetRequiredService<IDynamoDBClientFactory>();

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.TryAddDynamoDBFactories();

            return base.BuildServices(services);
        }
        protected DynamoDBClientFactorySpecification(DynamoDBConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper) 
            : base(configurationFixture, testOutputHelper)
        {
        }
    }

    public abstract class DynamoDBClientFactorySpecification<TResult> : SpecificationWithConfiguration<DynamoDBConfigurationFixture, TResult>
    {
        protected IDynamoDBClientFactory Factory => _serviceProvider.GetRequiredService<IDynamoDBClientFactory>();

        protected override IServiceCollection BuildServices(IServiceCollection services)
        {
            services.TryAddDynamoDBFactories();

            return base.BuildServices(services);
        }
        protected DynamoDBClientFactorySpecification(DynamoDBConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper)
            : base(configurationFixture, testOutputHelper)
        {
        }
    }
}
