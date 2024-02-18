using Testcontainers.PostgreSql;

namespace Sourcey.Integration.Tests.EntityFrameworkCore;

public class EventStoreDbFixture : IAsyncLifetime
{
    public readonly PostgreSqlContainer eventStore = new PostgreSqlBuilder().Build();

    public Task InitializeAsync() => eventStore.StartAsync();

    public async Task DisposeAsync() => await eventStore.DisposeAsync();
}
