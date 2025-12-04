using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.Extensions;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections;

internal sealed class ProjectionWriter<TProjection> : IProjectionWriter<TProjection>
    where TProjection : class, IProjection
{
    private readonly IWriteableProjectionDbContextFactory _projectionDbContextFactory;
    private readonly ILogger<ProjectionWriter<TProjection>> _logger;
    private readonly string _name;

    public ProjectionWriter(IWriteableProjectionDbContextFactory projectionDbContextFactory,
        ILogger<ProjectionWriter<TProjection>> logger)
    {
        if (projectionDbContextFactory == null)
            throw new ArgumentNullException(nameof(projectionDbContextFactory));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        _projectionDbContextFactory = projectionDbContextFactory;
        _logger = logger;

        _name = typeof(TProjection).FriendlyFullName();
    }

    public async Task<TProjection> AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(AddAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var entity = add();

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            await context.Set<TProjection>().AddAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


        return entity;
        }
    }

    public async Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(RemoveAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            var entity = await context.Set<TProjection>().FindAsync(new[] { subject }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (entity == null)
            return;

        context.Set<TProjection>().Remove(entity);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        TProjection entity;

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            entity = await context.Set<TProjection>().FindAsync(new[] { subject }, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (entity == null)
            return null;

        update(entity);

        context.Set<TProjection>().Update(entity);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


        return entity;
        }
    }

    public async Task<TProjection> UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var view = await UpdateAsync(subject, v =>
        {
            update(v);
            return v;
        }, cancellationToken).ConfigureAwait(false);

        return view;
    }

    public async Task<TProjection> AddOrUpdateAsync(string subject, Action<TProjection> update, Func<TProjection> create, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            var entity = await context.Set<TProjection>().FindAsync(new[] { subject }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (entity is not null)
        {
            update(entity);
            context.Set<TProjection>().Update(entity);
        }
        else
        {
            entity = create();
            await context.Set<TProjection>().AddAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return entity;
        }
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ProjectionWriter<TProjection>)}.{nameof(ResetAsync)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        var context = _projectionDbContextFactory.Create<TProjection>();
        await using (context.ConfigureAwait(false))
        {
            await context.Set<TProjection>().ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
