using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Events.DbContexts;

namespace Sourcey.EntityFrameworkCore.Events.Initializers;

internal sealed record EventStoreInitializerOptions<TStoreDbContext>(bool AutoMigrate)
    where TStoreDbContext : DbContext, IEventStoreDbContext;
