using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Stores;
using Zion.EntityFrameworkCore.Builder;
using Zion.EntityFrameworkCore.Commands.DbContexts;
using Zion.EntityFrameworkCore.Commands.Factories;
using Zion.EntityFrameworkCore.Commands.Stores;

namespace Zion.EntityFrameworkCore.Commands.Extensions
{
    public static class ZionEntityFrameworkCoreBuilderExtensions
    {
        public static IZionEntityFrameworkCoreBuilder AddCommands(this IZionEntityFrameworkCoreBuilder builder, Action<DbContextOptionsBuilder> options)
        {
            builder.Services.RemoveAll<ICommandStore>();
            builder.Services.TryAdd(GetCommandServices());
            builder.Services.AddDbContext<CommandStoreDbContext>(options);
            return builder;
        }

        private static IEnumerable<ServiceDescriptor> GetCommandServices()
        {
            yield return ServiceDescriptor.Scoped<ICommandStoreDbContextFactory, CommandStoreDbContextFactory>();
            yield return ServiceDescriptor.Scoped<ICommandStore, CommandStore>();
        }
    }
}
