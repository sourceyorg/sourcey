using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers
{
    internal class ProjectionInitializer<TProjection> : ISourceyInitializer
        where TProjection : class, IProjection
    {
        public bool ParallelEnabled => false;
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;
        private readonly ProjectionOptions<TProjection> _options;


        public ProjectionInitializer(IProjectionDbContextFactory projectionDbContextFactory,
            ProjectionOptions<TProjection> options)
        {
            if (projectionDbContextFactory is null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));
            if(options is null)
                throw new ArgumentNullException(nameof(options));

            _projectionDbContextFactory = projectionDbContextFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;
            
            using var context = _projectionDbContextFactory.Create<TProjection>();
            await context.Database.MigrateAsync();
        }
    }
}
