using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Zion.EntityFrameworkCore.Builder
{
    internal readonly struct ZionEntityFrameworkCoreMigratorBuilder : IZionEntityFrameworkCoreMigratorBuilder
    {
        public readonly IServiceCollection _services;

        public ZionEntityFrameworkCoreMigratorBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IZionEntityFrameworkCoreMigratorBuilder WithDbContext<TDbContext>(Action<DbContextOptionsBuilder> options) 
            where TDbContext : DbContext
        {
            _services.AddSingleton<IDesignTimeDbContextFactory<TDbContext>>(new DbContextFactory<TDbContext>(options));
            _services.AddDbContext<TDbContext>(options);
            return this;
        }

        internal class DbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
            where TDbContext : DbContext
        {
            private readonly Action<DbContextOptionsBuilder> _options;

            public DbContextFactory(Action<DbContextOptionsBuilder> options)
            {
                _options = options;
            }

            public TDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
                _options(optionsBuilder);

                return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object[] { optionsBuilder.Options });
            }
        }
    }
}
