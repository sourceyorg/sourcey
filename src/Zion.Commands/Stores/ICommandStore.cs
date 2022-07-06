namespace Zion.Commands.Stores
{
    public interface ICommandStore
    {
        Task SaveAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}
