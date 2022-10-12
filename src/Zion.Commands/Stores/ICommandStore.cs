namespace Zion.Commands.Stores
{
    public interface ICommandStore<TCommandStoreContext>
    {
        Task SaveAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
