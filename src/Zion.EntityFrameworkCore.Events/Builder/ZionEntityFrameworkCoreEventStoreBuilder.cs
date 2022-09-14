using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Events.DbContexts;
using Zion.Extensions;

namespace Zion.EntityFrameworkCore.Events.Builder
{
    internal readonly struct ZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext> : IZionEntityFrameworkCoreEventStoreBuilder<TEventStoreContext>
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        private readonly IZionEntityFrameworkCoreBuilder _parent;
        public readonly IServiceCollection Services => _parent.Services;

        public ZionEntityFrameworkCoreEventStoreBuilder(IZionEntityFrameworkCoreBuilder parent)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            _parent = parent;
        }

        public IZionEntityFrameworkCoreEventStoreBuilder<TNewEventStoreContext> AddEventStore<TNewEventStoreContext>(Action<DbContextOptionsBuilder> options)
            where TNewEventStoreContext : DbContext, IEventStoreDbContext
            => _parent.AddEventStore<TNewEventStoreContext>(options);
    }
}
