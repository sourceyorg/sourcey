using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sourcey.EntityFrameworkCore.Events.DbContexts;

namespace Sourcey.EntityFrameworkCore.Events.Builder;

public interface IEntityFrameworkCoreEventStoreBuilder<TEventStoreContext>
    where TEventStoreContext : DbContext, IEventStoreDbContext
{
    IServiceCollection Services { get; }

    IEntityFrameworkCoreEventStoreBuilder<TNewEventStoreContext> AddEventStore<TNewEventStoreContext>(Action<DbContextOptionsBuilder> options)
        where TNewEventStoreContext : DbContext, IEventStoreDbContext;
}
