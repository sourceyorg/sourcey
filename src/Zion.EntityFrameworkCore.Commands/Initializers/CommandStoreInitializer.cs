using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zion.Core.Initialization;
using Zion.EntityFrameworkCore.Commands.Factories;

namespace Zion.EntityFrameworkCore.Commands.Initializers
{
    internal sealed class CommandStoreInitializer : IZionInitializer
    {
        public bool ParallelEnabled => false;
        private readonly ICommandStoreDbContextFactory _dbContextFactory;
        private readonly CommandStoreOptions _options;


        public CommandStoreInitializer(ICommandStoreDbContextFactory dbContextFactory,
            CommandStoreOptions options)
        {
            if (dbContextFactory is null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _dbContextFactory = dbContextFactory;
            _options = options;
        }

        public async Task InitializeAsync(IHost host)
        {
            if (!_options.AutoMigrate)
                return;

            using var context = _dbContextFactory.Create();
            await context.Database.MigrateAsync();
        }
    }
}
