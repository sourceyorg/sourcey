using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Stores;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Commands.DbContexts;
using Zion.EntityFrameworkCore.Commands.Initializers;
using Zion.EntityFrameworkCore.Commands.Stores;

namespace Zion.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddCommands(
            this IZionEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            CommandStoreType storeType = CommandStoreType.Buffered,
            bool autoMigrate = true)
            => AddCommands<DefaultCommandStoreDbContext>(builder, options, storeType, autoMigrate);

        public static IZionEntityFrameworkCoreBuilder AddCommands<TCommandStoreDbContext>(
            this IZionEntityFrameworkCoreBuilder builder,
            Action<DbContextOptionsBuilder> options,
            CommandStoreType storeType = CommandStoreType.Buffered,
            bool autoMigrate = true)
            where TCommandStoreDbContext : CommandStoreDbContext
        {
            builder.Services.RemoveAll<ICommandStore<TCommandStoreDbContext>>();
            builder.Services.AddDbContext<TCommandStoreDbContext>(options);
            builder.Services.AddScoped<IZionInitializer, CommandStoreInitializer<TCommandStoreDbContext>>();
            builder.Services.AddSingleton(new CommandStoreOptions<TCommandStoreDbContext>(autoMigrate));

            if (storeType == CommandStoreType.Buffered)
                builder.RegisterBufferedCommandStore<TCommandStoreDbContext>();
            else
                builder.Services.TryAddSingleton<ICommandStore<TCommandStoreDbContext>, CommandStore<TCommandStoreDbContext>>();

            return builder;
        }

        private static void RegisterBufferedCommandStore<TCommandStoreDbContext>(this IZionEntityFrameworkCoreBuilder builder)
            where TCommandStoreDbContext : CommandStoreDbContext
        {
            builder.Services.TryAddSingleton<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>();
            builder.Services.TryAddSingleton<ICommandStore<TCommandStoreDbContext>>(sp => sp.GetRequiredService<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<EntityFrameworkCore.Commands.Stores.BufferedCommandStore<TCommandStoreDbContext>>());
        }
    }
}
