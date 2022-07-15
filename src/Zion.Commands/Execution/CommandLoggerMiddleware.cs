using Zion.Commands.Stores;

namespace Zion.Commands.Execution
{
    internal sealed class CommandStoreMiddleware<TCommand> : IPostCommandMiddleware<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandStore _commandStore;

        public CommandStoreMiddleware(ICommandStore commandStore)
        {
            if (commandStore is null)
                throw new ArgumentNullException(nameof(commandStore));
            
            _commandStore = commandStore;
        }

        public async Task<bool> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            await _commandStore.SaveAsync(command, cancellationToken);
            return true;
        }
    }
}
