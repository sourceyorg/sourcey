using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Sourcey.EntityFrameworkCore.Projections;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;
using Sourcey.Keys;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Tests.Projections.Reader;

public class WhenUsingInMemoryContext
{
    private sealed class DummyProjection : IProjection
    {
        public string Subject { get; set; } = string.Empty;
        public string? Value { get; set; }
    }

    private sealed class TestProjectionDbContext : DbContext
    {
        public TestProjectionDbContext(DbContextOptions<TestProjectionDbContext> options) : base(options) { }

        public DbSet<DummyProjection> Projections => Set<DummyProjection>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DummyProjection>(b =>
            {
                b.HasKey(x => x.Subject);
                b.Property(x => x.Subject).IsRequired();
            });
        }
    }

    private sealed class InMemoryReadonlyFactory : IReadonlyProjectionDbContextFactory
    {
        private readonly string _dbName;
        public InMemoryReadonlyFactory(string dbName) => _dbName = dbName;

        public DbContext Create<TProjection>() where TProjection : class, IProjection
        {
            var options = new DbContextOptionsBuilder<TestProjectionDbContext>()
                .UseInMemoryDatabase(_dbName)
                .Options;
            return new TestProjectionDbContext(options);
        }
    }

    private static ProjectionReader<DummyProjection> Sut(string dbName)
    {
        var factory = new InMemoryReadonlyFactory(dbName);
        var logger = NullLogger<ProjectionReader<DummyProjection>>.Instance;
        return new ProjectionReader<DummyProjection>(logger, factory);
    }

    private static TestProjectionDbContext NewCtx(string dbName)
        => new(new DbContextOptionsBuilder<TestProjectionDbContext>().UseInMemoryDatabase(dbName).Options);

    [Then]
    public async Task ReadAsync_returns_entity_when_found()
    {
        var db = Guid.NewGuid().ToString();
        using (var seed = NewCtx(db))
        {
            seed.Projections.Add(new DummyProjection { Subject = "s1", Value = "A" });
            await seed.SaveChangesAsync();
        }

        var sut = Sut(db);
        var found = await sut.ReadAsync(Subject.From("s1"), cancellationToken: default);
        found.ShouldNotBeNull();
        found!.Value.ShouldBe("A");
    }

    [Then]
    public async Task ReadAsync_returns_null_when_not_found()
    {
        var sut = Sut(Guid.NewGuid().ToString());
        var found = await sut.ReadAsync(Subject.From("missing"), cancellationToken: default);
        found.ShouldBeNull();
    }

    [Then]
    public async Task ReadAsync_selector_projects_value()
    {
        var db = Guid.NewGuid().ToString();
        using (var seed = NewCtx(db))
        {
            seed.Projections.Add(new DummyProjection { Subject = "s1", Value = "A" });
            await seed.SaveChangesAsync();
        }

        var sut = Sut(db);
        Expression<Func<DummyProjection, string?>> selector = p => p.Value;
        var value = await sut.ReadAsync(Subject.From("s1"), selector, cancellationToken: default);
        value.ShouldBe("A");
    }

    [Then]
    public async Task Consistency_check_succeeds_within_retries()
    {
        var db = Guid.NewGuid().ToString();
        var sut = Sut(db);

        var subject = Subject.From("s1");
        var checkTask = sut.ReadAsync(subject, s => s != null && s.Subject == subject, retryCount: 20, delay: TimeSpan.FromMilliseconds(10));

        // Seed after starting the check
        using (var seed = NewCtx(db))
        {
            seed.Projections.Add(new DummyProjection { Subject = subject, Value = "A" });
            await seed.SaveChangesAsync();
        }

        var result = await checkTask;
        result.ShouldNotBeNull();
        result!.Subject.ShouldBe(subject);
    }

    [Then]
    public async Task Consistency_check_failure_returns_default()
    {
        var db = Guid.NewGuid().ToString();
        var sut = Sut(db);
        var subject = Subject.From("s2");

        var result = await sut.ReadAsync(subject, s => s != null, retryCount: 2, delay: TimeSpan.FromMilliseconds(5));
        result.ShouldBeNull();
    }

    private sealed class ThrowingReadonlyFactory : IReadonlyProjectionDbContextFactory
    {
        public DbContext Create<TProjection>() where TProjection : class, IProjection
            => throw new NotImplementedException("Should not be invoked when token is pre-cancelled");
    }

    private static CancellationToken Cancelled()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts.Token;
    }

    [Then]
    public async Task Cancellation_guard_on_ReadAsync_entity()
    {
        var factory = new ThrowingReadonlyFactory();
        var logger = NullLogger<ProjectionReader<DummyProjection>>.Instance;
        var sut = new ProjectionReader<DummyProjection>(logger, factory);

        await Should.ThrowAsync<OperationCanceledException>(async () => await sut.ReadAsync(Subject.From("s"), Cancelled()));
    }

    [Then]
    public async Task Cancellation_guard_on_ReadAsync_selector()
    {
        var factory = new ThrowingReadonlyFactory();
        var logger = NullLogger<ProjectionReader<DummyProjection>>.Instance;
        var sut = new ProjectionReader<DummyProjection>(logger, factory);

        Expression<Func<DummyProjection, string?>> selector = p => p.Value;
        await Should.ThrowAsync<OperationCanceledException>(async () => await sut.ReadAsync(Subject.From("s"), selector, Cancelled()));
    }
}
