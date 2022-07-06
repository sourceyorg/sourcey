using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Events.DbContexts;

namespace Zion.EntityFrameworkCore.Events.Factories
{
    internal sealed class EventStoreDbContextFactory<TEventStoreDbContext> : IEventStoreDbContextFactory<TEventStoreDbContext>
        where TEventStoreDbContext : DbContext, IEventStoreDbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public EventStoreDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public TEventStoreDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<TEventStoreDbContext>>();
            return (TEventStoreDbContext)Activator.CreateInstance(typeof(TEventStoreDbContext), new object[] { options });
        }
    }
}
