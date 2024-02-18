using Testcontainers.PostgreSql;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public class ProjectionsDbFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer projections = new PostgreSqlBuilder().Build();

    public Task InitializeAsync() => projections.StartAsync();

    public async Task DisposeAsync() => await projections.DisposeAsync();
}
