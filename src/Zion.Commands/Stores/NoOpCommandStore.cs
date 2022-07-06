namespace Zion.Commands.Stores
{
    internal sealed class NoOpCommandStore : ICommandStore
    {
        public Task SaveAsync(ICommand command, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
