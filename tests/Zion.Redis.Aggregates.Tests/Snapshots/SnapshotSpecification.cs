using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Zion.Aggregates;
using Zion.Extensions;

namespace Zion.Redis.Aggregates.Tests.Snapshots
{
    public abstract class SnapshotSpecification<TAggregate, TAggregateState> : Specification
        where TAggregate : Aggregate<TAggregateState>
        where TAggregateState : IAggregateState, new()
    {
        protected readonly RedisTestcontainer _redisContainer = new TestcontainersBuilder<RedisTestcontainer>()
            .WithDatabase(new RedisTestcontainerConfiguration())
            .Build();
        
        protected override void BuildServices(IServiceCollection services)
        {
            services.AddZion()
                .AddJsonSerialization(o => o.AddAggregateSerialization())
                .AddAggregate<TAggregate, TAggregateState>(a => 
                a.WithRedisSnapshotStrategy(o 
                    => o.ConnectionString = _redisContainer.ConnectionString));

            base.BuildServices(services);
        }

        protected SnapshotSpecification(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        public override async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();
            await base.InitializeAsync();
        }

        public override async Task DisposeAsync()
        {
            await _redisContainer.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}
