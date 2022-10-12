using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Stores;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Commands.DbContexts;
using Zion.EntityFrameworkCore.Commands.Initializers;
using Zion.EntityFrameworkCore.Commands.Stores;
using BufferedCommandStore = Zion.EntityFrameworkCore.Commands.Stores.BufferedCommandStore;

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
            builder.Services.RemoveAll<ICommandStore>();
            builder.Services.AddDbContext<TCommandStoreDbContext>(options);
            builder.Services.TryAddScoped<CommandStoreDbContext>(s => s.GetRequiredService<TCommandStoreDbContext>());
            builder.Services.AddScoped<IZionInitializer, CommandStoreInitializer>();
            builder.Services.AddSingleton(new CommandStoreOptions(autoMigrate));

            if (storeType == CommandStoreType.Buffered)
                builder.RegisterBufferedCommandStore();
            else
                builder.Services.TryAddSingleton<ICommandStore, CommandStore>();

            return builder;
        }

        private static void RegisterBufferedCommandStore(this IZionEntityFrameworkCoreBuilder builder)
        {
            builder.Services.TryAddSingleton<BufferedCommandStore>();
            builder.Services.TryAddSingleton<ICommandStore>(sp => sp.GetRequiredService<BufferedCommandStore>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<BufferedCommandStore>());
        }
    }
}
