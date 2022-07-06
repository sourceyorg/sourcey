using Microsoft.Extensions.Logging;
using Zion.Commands;
using Zion.Commands.Serialization;
using Zion.Commands.Stores;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Commands.Factories;

namespace Zion.EntityFrameworkCore.Commands.Stores
{
    internal sealed class CommandStore : ICommandStore
    {
        private readonly ICommandStoreDbContextFactory _dbContextFactory;
        private readonly ICommandSerializer _commandSerializer;
        private readonly ILogger<CommandStore> _logger;

        public CommandStore(ICommandStoreDbContextFactory dbContextFactory,
                                               ICommandSerializer commandSerializer,
                                               ILogger<CommandStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (commandSerializer == null)
                throw new ArgumentNullException(nameof(commandSerializer));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _commandSerializer = commandSerializer;
            _logger = logger;
        }

        public async Task SaveAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var type = command.GetType();
            var data = _commandSerializer.Serialize(command);

            using (var context = _dbContextFactory.Create())
            {
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
                });

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
