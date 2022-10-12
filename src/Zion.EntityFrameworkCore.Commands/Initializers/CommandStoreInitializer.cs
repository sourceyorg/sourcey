using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Commands.DbContexts;

namespace Zion.EntityFrameworkCore.Commands.Initializers
{
    internal sealed class CommandStoreInitializer<TCommandStoreDbContext> : IZionInitializer
        where TCommandStoreDbContext : CommandStoreDbContext
    {
        public bool ParallelEnabled => false;
        private readonly CommandStoreOptions<TCommandStoreDbContext> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public CommandStoreInitializer(IServiceScopeFactory serviceScopeFactory,
            CommandStoreOptions<TCommandStoreDbContext> options)
        {
            if (serviceScopeFactory is null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;

            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TCommandStoreDbContext>();

            await context.Database.MigrateAsync();
        }
    }
}
