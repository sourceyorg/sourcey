using Microsoft.Extensions.DependencyInjection;
using Sourcey.Commands;
using Sourcey.Commands.Serialization;
using Sourcey.Commands.Stores;
using Sourcey.Core.Extensions;
using Sourcey.EntityFrameworkCore.Commands.DbContexts;

namespace Sourcey.EntityFrameworkCore.Commands.Stores
{
    internal sealed class CommandStore<TCommandStoreDbContext> : ICommandStore<TCommandStoreDbContext>
        where TCommandStoreDbContext : CommandStoreDbContext
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICommandSerializer _commandSerializer;

        public CommandStore(IServiceScopeFactory serviceScopeFactory,
            ICommandSerializer commandSerializer)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (commandSerializer == null)
                throw new ArgumentNullException(nameof(commandSerializer));

            _serviceScopeFactory = serviceScopeFactory;
            _commandSerializer = commandSerializer;
        }

        public async Task SaveAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var type = command.GetType();
            var data = _commandSerializer.Serialize(command);

            using var scope = _serviceScopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<TCommandStoreDbContext>();

            await context.Commands.AddAsync(new Entities.Command
            {
                Name = type.Name,
                Type = type.FriendlyFullName(),
                Subject = command.Subject,
                Correlation = command.Correlation,
                Data = data,
                Id = command.Id,
                Actor = command.Actor,
                Timestamp = command.Timestamp,
            }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
