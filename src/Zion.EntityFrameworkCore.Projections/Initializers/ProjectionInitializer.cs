using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Initializers
{
    internal class ProjectionInitializer<TProjection> : IZionInitializer
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
            if (!_options._autoMigrate)
                return;
            
            using var context = _projectionDbContextFactory.Create<TProjection>();
            await context.Database.MigrateAsync();
        }
    }
}
