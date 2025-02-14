using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;

public abstract class DbContextFactory<TDbContext, TDbType> : IDbContextFactory<TDbContext>
    where TDbContext : DbContext
    where TDbType : DbType
{
    private delegate TDbContext? CreateDbContext(object? instance);

    private static readonly ConcurrentDictionary<Type, CreateDbContext> _createDbContextMethods = new();

    // ReSharper disable once StaticMemberInGenericType
    private static readonly Type _factoryType = typeof(Microsoft.EntityFrameworkCore.IDbContextFactory<>);

    private static TDbContext? BuildContext(object? instance)
    {
        if (instance is null)
            return null;

        var type = instance.GetType();

        if (_createDbContextMethods.TryGetValue(type, out var method)) 
            return method(instance);
        
        method = x => (TDbContext?)type.GetMethod("CreateDbContext")?.Invoke(x, []);
        _createDbContextMethods.TryAdd(type, method);

        return method(instance);
    }

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
        var contextFactory = _serviceProvider.GetService(_factoryType.MakeGenericType(types.ContextType));

        if (contextFactory is not null)
        {
            return BuildContext(contextFactory);
        }

        var options = _serviceProvider.GetRequiredService(types.OptionsType);
        return (TDbContext?)Activator.CreateInstance(types.ContextType, [options]);
    }
}
