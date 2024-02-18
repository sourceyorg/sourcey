using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Sourcey.Extensions;
using Sourcey.Initialization;
using Sourcey.Testing.Integration.Abstractions;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Events;
using Sourcey.Testing.Integration.Stubs.Projections;
using Sourcey.Testing.Integration.Stubs.Projections.Managers;
using Testcontainers.PostgreSql;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;


[CollectionDefinition(nameof(PostgreContainerCollection))]
public class PostgreContainerCollection : ICollectionFixture<ProjectionsDbFixture>, ICollectionFixture<EventStoreDbFixture>;

public class ProjectionsDbFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer projections = new PostgreSqlBuilder().Build();

    public Task InitializeAsync() => projections.StartAsync();

    public async Task DisposeAsync() => await projections.DisposeAsync();
}

public class EventStoreDbFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer eventStore = new PostgreSqlBuilder().Build();

    public Task InitializeAsync() => eventStore.StartAsync();

    public async Task DisposeAsync() => await eventStore.DisposeAsync();
}

public class EntityFrameworkCoreWebApplicationFactory: SourceyWebApplicationFactory
{
    public ProjectionsDbFixture projections;
    public EventStoreDbFixture eventStore;

    private sealed class DbInitializer : ISourceyInitializer
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly IDbContextFactory<EventStoreDbContext> _eventStoreDbContextFactory;

        public DbInitializer(IProjectionDbContextFactory projectionDbContextFactory, IDbContextFactory<EventStoreDbContext> eventStoreDbContextFactory)
        {
            _projectionDbContextFactory = projectionDbContextFactory;
            _eventStoreDbContextFactory = eventStoreDbContextFactory;
        }

        public bool ParallelEnabled => false;
        
        public async Task InitializeAsync(IHost host)
        {
            await using (var dbContext = _projectionDbContextFactory.Create<Something>())
            {
                await dbContext.Database.EnsureCreatedAsync();
            }

            await using (var dbContext = await _eventStoreDbContextFactory.CreateDbContextAsync())
            {
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }   
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<ISourceyInitializer, DbInitializer>();
            services.AddSourcey(builder =>
            {
                builder.AddAggregate<SampleAggreagte, SampleState>();

                builder.AddEvents(e =>
                {
                    e.RegisterEventCache<SomethingHappened>();
                    e.WithEntityFrameworkCoreEventStore<EventStoreDbContext>(x =>
                        {
                            x.AddAggregate<SampleAggreagte, SampleState>();
                            x.AddProjection<Something>(p => p.WithInterval(1));
                        },
                        o =>
                        {
                            o.UseNpgsql(
                                eventStore.eventStore.GetConnectionString()
                            );
                        });
                });

                builder.AddProjection<Something>(x =>
                {
                    x.WithManager<SomethingManager>();
                    x.WithEntityFrameworkCoreWriter(e =>
                    {
                        e.WithContext<SomethingContext>(o => o.UseNpgsql(
                            projections.projections.GetConnectionString()
                        ));
                    });
                    x.WithEntityFrameworkCoreStateManager(e =>
                    {
                        e.WithContext<SomethingContext>(o => o.UseNpgsql(
                            projections.projections.GetConnectionString()
                        ));
                    });
                });
             
                 builder.AddSerialization(x =>
                 {
                     x.WithEvents();
                     x.WithAggregates();
                 });
            });  
        });
         
         base.ConfigureWebHost(builder);
    }
}
