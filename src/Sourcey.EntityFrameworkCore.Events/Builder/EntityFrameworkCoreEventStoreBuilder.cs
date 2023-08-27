using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.EntityFrameworkCore.Builder;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Extensions;

namespace Sourcey.EntityFrameworkCore.Events.Builder;

internal readonly struct EntityFrameworkCoreEventStoreBuilder<TEventStoreContext> : IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext>
    where TEventStoreContext : DbContext, IEventStoreDbContext
{
    private readonly IEntityFrameworkCoreBuilder _parent;
    public readonly IServiceCollection Services => _parent.Services;

    public EntityFrameworkCoreEventStoreBuilder(IEntityFrameworkCoreBuilder parent)
    {
        if (parent == null)
            throw new ArgumentNullException(nameof(parent));

        _parent = parent;
    }

    public IEntityFrameworkCoreEventStoreBuilder<TNewEventStoreContext> AddEventStore<TNewEventStoreContext>(Action<DbContextOptionsBuilder> options)
        where TNewEventStoreContext : DbContext, IEventStoreDbContext
        => _parent.AddEventStore<TNewEventStoreContext>(options);
}
