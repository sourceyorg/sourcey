using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.Extensions;
using Sourcey.Initialization;
using Sourcey.Integration.Tests.EntityFrameworkCore.Projections;
using Sourcey.Testing.Integration.Abstractions;
using Sourcey.Testing.Integration.Stubs.Aggregates;
using Sourcey.Testing.Integration.Stubs.Events;
using Sourcey.Testing.Integration.Stubs.Projections;
using Sourcey.Testing.Integration.Stubs.Projections.Managers;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public class EntityFrameworkCoreWebApplicationFactory : SourceyWebApplicationFactory
{
    public HostFixture HostFixture { get; set; }

    private sealed class DbInitializer : ISourceyInitializer
    {
        private readonly IWriteableProjectionDbContextFactory _projectionDbContextFactory;
        private readonly IDbContextFactory<EventStoreDbContext> _eventStoreDbContextFactory;

        public DbInitializer(IWriteableProjectionDbContextFactory projectionDbContextFactory,
            IDbContextFactory<EventStoreDbContext> eventStoreDbContextFactory)
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
        builder.ConfigureLogging(logging =>
            logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning));
        builder.ConfigureServices(services =>
        {
            services.AddScoped<ISourceyInitializer, DbInitializer>();
            services.AddDbContextFactory<WriteableSomethingContext>(o => o.UseNpgsql(
                HostFixture.ProjectionsConnectionString
            ));
            services.AddPooledDbContextFactory<ReadonlySomethingContext>(o => o.UseNpgsql(
                HostFixture.ProjectionsConnectionString
            ));
            services.AddDbContextFactory<EventStoreDbContext>(o => o.UseNpgsql(
                HostFixture.EventStoreConnectionString
            ));

            services.AddSourcey(builder =>
            {
                builder.AddAggregate<SampleAggregate, SampleState>();

                builder.AddEvents(e =>
                {
                    e.RegisterEventCache<SomethingHappened>();
                    e.WithEntityFrameworkCoreEventStore<EventStoreDbContext>(x =>
                    {
                        x.AddAggregate<SampleAggregate, SampleState>();
                        x.AddProjection<Something>(p => p.WithInterval(1).WithPageSize(10_000));
                    });
                });

                builder.AddProjection<Something>(x =>
                {
                    x.WithManager<SomethingManager>();
                    x.WithEntityFrameworkCoreWriter(e => e.WithContext<WriteableSomethingContext>());
                    x.WithEntityFrameworkCoreReader(e => e.WithContext<ReadonlySomethingContext>());
                    x.WithEntityFrameworkCoreStateManager(e => e.WithContext<WriteableSomethingContext>());
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
