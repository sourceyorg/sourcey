using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Projections.DbContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Factories
{
    internal sealed class ProjectionDbContextFactory : IProjectionDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProjectionDbContextTypesFactory _projectionDbContextTypesFactory;

        public ProjectionDbContextFactory(IServiceProvider serviceProvider,
            IProjectionDbContextTypesFactory projectionDbContextTypesFactory)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (projectionDbContextTypesFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextTypesFactory));

            _serviceProvider = serviceProvider;
            _projectionDbContextTypesFactory = projectionDbContextTypesFactory;
        }

        public ProjectionDbContext Create<TProjection>()
            where TProjection : class, IProjection
        {
            var types = _projectionDbContextTypesFactory.Create<TProjection>();
            var options = _serviceProvider.GetRequiredService(types.OptionsType);

            return (ProjectionDbContext)Activator.CreateInstance(types.ContextType, new object[] { options });
        }
    }
}
