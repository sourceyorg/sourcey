using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Sourcey.EntityFrameworkCore.Projections;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Tests.Projections.Writer;

public class When_cancellation_is_requested
{
    private sealed class DummyProjection : IProjection
    {
        public string Subject { get; set; } = string.Empty;
    }

    private sealed class ThrowingWriteableFactory : IWriteableProjectionDbContextFactory
    {
        public DbContext Create<TProjection>() where TProjection : class, IProjection
            => throw new NotImplementedException("Should not be called when token is pre-cancelled");
    }

    private static ProjectionWriter<DummyProjection> Sut()
    {
        var factory = new ThrowingWriteableFactory();
        var logger = NullLogger<ProjectionWriter<DummyProjection>>.Instance;
        return new ProjectionWriter<DummyProjection>(factory, logger);
    }

    private static CancellationToken Cancelled()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        return cts.Token;
    }

    [Then]
    public async Task AddAsync_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.AddAsync("s", () => new DummyProjection { Subject = "s" }, Cancelled()));
    }

    [Then]
    public async Task RemoveAsync_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.RemoveAsync("s", Cancelled()));
    }

    [Then]
    public async Task UpdateAsync_Func_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.UpdateAsync("s", v => v, Cancelled()));
    }

    [Then]
    public async Task UpdateAsync_Action_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.UpdateAsync("s", _ => { }, Cancelled()));
    }

    [Then]
    public async Task AddOrUpdateAsync_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.AddOrUpdateAsync("s", _ => { }, () => new DummyProjection { Subject = "s" }, Cancelled()));
    }

    [Then]
    public async Task ResetAsync_throws_when_cancelled()
    {
        var sut = Sut();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await sut.ResetAsync(Cancelled()));
    }
}
