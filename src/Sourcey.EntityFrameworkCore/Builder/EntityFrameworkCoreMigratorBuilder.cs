using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.EntityFrameworkCore.Builder;

internal readonly struct EntityFrameworkCoreMigratorBuilder : IEntityFrameworkCoreMigratorBuilder
{
    public readonly IServiceCollection _services;

    public EntityFrameworkCoreMigratorBuilder(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        _services = services;
    }

    public IEntityFrameworkCoreMigratorBuilder WithDbContext<TDbContext>(Action<DbContextOptionsBuilder> options) 
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
