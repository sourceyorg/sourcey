using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zion.EntityFrameworkCore.Commands.DbContexts;

namespace Zion.EntityFrameworkCore.Commands.Factories
{
    internal sealed class CommandStoreDbContextFactory : ICommandStoreDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandStoreDbContextFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public CommandStoreDbContext Create()
        {
            var options = _serviceProvider.GetRequiredService<DbContextOptions<CommandStoreDbContext>>();
            return new CommandStoreDbContext(options);
        }
    }
}
