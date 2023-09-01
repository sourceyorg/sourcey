using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.Builder;
using Sourcey.EntityFrameworkCore.Events.DbContexts;
using Sourcey.Events.Builder;

namespace Sourcey.Extensions;

public static class EventsBuilderExtensions
{
    public static IEventsBuilder WithEntityFrameworkCoreEventStore<TEventStoreContext>(
        this IEventsBuilder builder,
        Action<IEventStoreBuilder> action,
        Action<DbContextOptionsBuilder> options,
        bool autoMigrate = true)
        where TEventStoreContext : DbContext, IEventStoreDbContext
    {
        action(new EntityFrameworkCoreEventStoreBuilder<TEventStoreContext>(builder.Services, options, autoMigrate));
        return builder;
    }
}