using Microsoft.Extensions.Logging;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Projections.Entities;
using Zion.EntityFrameworkCore.Projections.Factories;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections
{
    internal sealed class ProjectionStateManager<TProjection> : IProjectionStateManager<TProjection>
        where TProjection : class, IProjection
    {
        private readonly ILogger<ProjectionStateManager<TProjection>> _logger;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly string _key;

        public ProjectionStateManager(ILogger<ProjectionStateManager<TProjection>> logger,
            IProjectionDbContextFactory projectionDbContextFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _logger = logger;
            _projectionDbContextFactory = projectionDbContextFactory;
            _key = $"ProjectionState_{typeof(TProjection).FriendlyName()}";
        }
        
        public async Task RemoveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(RemoveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionDbContextFactory.Create<TProjection>();
            var state = await context.ProjectionStates.FindAsync(new { Key = _key }, cancellationToken);
            
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

            using var context = _projectionDbContextFactory.Create<TProjection>();
            return await context.ProjectionStates.FindAsync(new { Key = _key }, cancellationToken);
        }

        public async Task<IProjectionState> UpdateAsync(Action<IProjectionState>? update, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionStateManager<TProjection>)}.{nameof(UpdateAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            using var context = _projectionDbContextFactory.Create<TProjection>();

            var entity = await context.ProjectionStates.FindAsync(new { Key = _key }, cancellationToken);
            
            if(entity is null)
            {
                entity = new ProjectionState
                {
                    Key = _key,
                    CreatedDate = DateTimeOffset.UtcNow,
                    Position = 1
                };

                context.Add(entity);
            }

            update?.Invoke(entity);
            context.Update(entity);
            
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }
    }
}
