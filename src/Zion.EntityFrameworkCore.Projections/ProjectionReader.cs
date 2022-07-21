using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zion.Core.Keys;
using Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections
{
    internal sealed class ProjectionReader<TProjection> : IProjectionReader<TProjection>
        where TProjection : class, IProjection
    {
        private readonly ILogger<ProjectionReader<TProjection>> _logger;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;


        public ProjectionReader(ILogger<ProjectionReader<TProjection>> logger,
            IProjectionDbContextFactory projectionDbContextFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _logger = logger;
            _projectionDbContextFactory = projectionDbContextFactory;
        }

        public async Task<TProjection?> RetrieveAsync(Subject subject, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(RetrieveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            using var context = _projectionDbContextFactory.Create<TProjection>();
            return await context.Set<TProjection>().FindAsync(new object?[] { subject.ToString() }, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<TProjection>> RetrieveAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(ProjectionReader<TProjection>)}.{nameof(RetrieveAsync)} was cancelled before execution");
                cancellationToken.ThrowIfCancellationRequested();
            }

            using var context = _projectionDbContextFactory.Create<TProjection>();
            return await context.Set<TProjection>().ToArrayAsync(cancellationToken);
        }
    }
}
