using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Events.Builder
{
    public interface IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext>
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        IServiceCollection Services { get; }

        IZionEntityFrameworkCoreEventStoreBuilder<TNewEventStoreContext> AddEventStore<TNewEventStoreContext>(Action<DbContextOptionsBuilder> options)
            where TNewEventStoreContext : DbContext, IEventStoreDbContext;
    }
}
