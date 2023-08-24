using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Commands.Stores;
using Sourcey.Core.Initialization;
using Sourcey.EntityFrameworkCore.Builder;
using Sourcey.EntityFrameworkCore.Commands.DbContexts;
using Sourcey.EntityFrameworkCore.Commands.Initializers;
using Sourcey.EntityFrameworkCore.Commands.Stores;

namespace Sourcey.Extensions
{
    public static class EntityFrameworkCoreBuilderExtensions
    {
        public static IEntityFrameworkCoreBuilder AddCommands(
            this IEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            CommandStoreType storeType = CommandStoreType.Buffered,
            bool autoMigrate = true)
            => AddCommands<DefaultCommandStoreDbContext>(builder, options, storeType, autoMigrate);

        public static IEntityFrameworkCoreBuilder AddCommands<TCommandStoreDbContext>(
            this IEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            CommandStoreType storeType = CommandStoreType.Buffered,
            bool autoMigrate = true)
            where TCommandStoreDbContext : CommandStoreDbContext
        {
            builder.Services.RemoveAll<ICommandStore<TCommandStoreDbContext>>();
            builder.Services.AddDbContext<TCommandStoreDbContext>(options);
            builder.Services.AddScoped<ISourceyInitializer, CommandStoreInitializer<TCommandStoreDbContext>>();
            builder.Services.AddSingleton(new CommandStoreOptions<TCommandStoreDbContext>(autoMigrate));

            if (storeType == CommandStoreType.Buffered)
                builder.RegisterBufferedCommandStore<TCommandStoreDbContext>();
            else
                builder.Services.TryAddSingleton<ICommandStore<TCommandStoreDbContext>, CommandStore<TCommandStoreDbContext>>();

            return builder;
        }

        private static void RegisterBufferedCommandStore<TCommandStoreDbContext>(this IEntityFrameworkCoreBuilder builder)
            where TCommandStoreDbContext : CommandStoreDbContext
        {
            builder.Services.TryAddSingleton<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>();
            builder.Services.TryAddSingleton<ICommandStore<TCommandStoreDbContext>>(sp => sp.GetRequiredService<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>());
        }
    }
}
