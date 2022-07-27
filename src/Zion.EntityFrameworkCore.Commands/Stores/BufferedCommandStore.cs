using Microsoft.Extensions.DependencyInjection;
using Zion.Commands;
using Zion.Commands.Serialization;
using Zion.Commands.Stores;
using Zion.Core.Extensions;
using Zion.EntityFrameworkCore.Commands.Factories;

namespace Zion.EntityFrameworkCore.Commands.Stores
{
    internal sealed class BufferedCommandStore : Zion.Commands.Stores.BufferedCommandStore
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ICommandSerializer _commandSerializer;

        public BufferedCommandStore(IServiceScopeFactory serviceScopeFactory,
            ICommandSerializer commandSerializer)
        {
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            if (commandSerializer == null)
                throw new ArgumentNullException(nameof(commandSerializer));

            _serviceScopeFactory = serviceScopeFactory;
            _commandSerializer = commandSerializer;
        }

        protected override async Task ConsumeAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var type = command.GetType();
            var data = _commandSerializer.Serialize(command);

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<ICommandStoreDbContextFactory>();

            using var context = dbContextFactory.Create();
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
