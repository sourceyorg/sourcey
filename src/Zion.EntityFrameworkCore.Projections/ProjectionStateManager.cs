using Microsoft.Extensions.Logging;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections
{
    internal sealed class ProjectionStateManager<TProjection> : IProjectionStateManager<TProjection>
        where TProjection : class, IProjection
    {
        private readonly ILogger<ProjectionStateManager<TProjection>> _logger;
        private readonly IProjectionStateDbContextFactory _projectionStateDbContextFactory;
        private readonly string _key;

        public ProjectionStateManager(ILogger<ProjectionStateManager<TProjection>> logger,
            IProjectionStateDbContextFactory projectionStateDbContextFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (projectionStateDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionStateDbContextFactory));

            _logger = logger;
            _projectionStateDbContextFactory = projectionStateDbContextFactory;
            _key = $"ProjectionState_{typeof(TProjection).FriendlyFullName()}";
        }
        
        public async Task RemoveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(RemoveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionStateDbContextFactory.Create<TProjection>();
            var state = await context.Set<ProjectionState>().FindAsync(new object[] { _key }, cancellationToken: cancellationToken);
            
            if (state is not null)
                context.Remove(state);
            
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IProjectionState?> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(RetrieveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionStateDbContextFactory.Create<TProjection>();
            return await context.Set<ProjectionState>().FindAsync(new object[] { _key }, cancellationToken: cancellationToken);
        }

        public async Task<IProjectionState> UpdateAsync(Action<IProjectionState> update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            using var context = _projectionStateDbContextFactory.Create<TProjection>();

            var entity = await context.Set<ProjectionState>().FindAsync(new object[] { _key }, cancellationToken: cancellationToken);

            if (entity is null)
                throw new InvalidOperationException("Missing state for projection");

            update(entity);
            context.Update(entity);
            
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public async Task<IProjectionState> CreateAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionStateDbContextFactory.Create<TProjection>();

            var entity = new ProjectionState
            {
                Key = _key,
                CreatedDate = DateTimeOffset.UtcNow,
                Position = 1
            };

            context.Add(entity);

            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}
