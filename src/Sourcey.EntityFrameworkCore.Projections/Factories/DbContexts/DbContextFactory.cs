using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts
{
    public abstract class DbContextFactory<TDbContext, TDbType> : IDbContextFactory<TDbContext>
        where TDbContext : DbContext
        where TDbType : DbType
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDbTypeFactory<TDbType> _dbTypeFactory;

        public DbContextFactory(IServiceProvider serviceProvider,
            IDbTypeFactory<TDbType> dbTypeFactory)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            if (dbTypeFactory == null)
                throw new ArgumentNullException(nameof(dbTypeFactory));

            _serviceProvider = serviceProvider;
            _dbTypeFactory = dbTypeFactory;
        }

        public TDbContext? Create<TProjection>()
            where TProjection : class, IProjection
        {
            var types = _dbTypeFactory.Create<TProjection>();
            var options = _serviceProvider.GetRequiredService(types.OptionsType);

            return (TDbContext?)Activator.CreateInstance(types.ContextType, new object[] { options });
        }
    }
}
