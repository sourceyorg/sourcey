using Sourcey.Commands.Stores;

namespace Sourcey.Commands.Execution
{
    internal sealed class CommandStoreMiddleware<TCommand, TCommandStoreContext> : IPostCommandMiddleware<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandStore<TCommandStoreContext> _commandStore;

        public CommandStoreMiddleware(ICommandStore<TCommandStoreContext> commandStore)
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
