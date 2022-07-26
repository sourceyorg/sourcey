using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Stores;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Commands.DbContexts;
using Zion.EntityFrameworkCore.Commands.Factories;
using Zion.EntityFrameworkCore.Commands.Initializers;
using Zion.EntityFrameworkCore.Commands.Stores;

namespace Zion.EntityFrameworkCore.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddCommands(this IZionEntityFrameworkCoreBuilder builder, Action<DbContextOptionsBuilder> options, bool autoMigrate = true)
        {
            builder.Services.RemoveAll<ICommandStore>();
            builder.Services.TryAddScoped<ICommandStoreDbContextFactory, CommandStoreDbContextFactory>();
            builder.Services.TryAddSingleton<CommandStore>();
            builder.Services.TryAddSingleton<ICommandStore>(sp => sp.GetRequiredService<CommandStore>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<CommandStore>());
            builder.Services.AddDbContext<CommandStoreDbContext>(options);
            builder.Services.AddScoped<IZionInitializer, CommandStoreInitializer>();
            builder.Services.AddSingleton(new CommandStoreOptions(autoMigrate));
            return builder;
        }
    }
}
