using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers
{
    internal class ProjectionStateInitializer<TProjection> : ISourceyInitializer
        where TProjection : class, IProjection
    {
        public bool ParallelEnabled => false;
        private readonly IProjectionStateDbContextFactory _projectionStateDbContextFactory;
        private readonly ProjectionStateOptions<TProjection> _options;


        public ProjectionStateInitializer(IProjectionStateDbContextFactory projectionStateDbContextFactory,
            ProjectionStateOptions<TProjection> options)
        {
            if (projectionStateDbContextFactory is null)
                throw new ArgumentNullException(nameof(projectionStateDbContextFactory));

            _projectionStateDbContextFactory = projectionStateDbContextFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;
            
            using var context = _projectionStateDbContextFactory.Create<TProjection>();
            await context.Database.MigrateAsync();
        }
    }
}
