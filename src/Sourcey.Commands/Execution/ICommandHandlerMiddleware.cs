namespace Sourcey.Commands.Execution
{
    public interface ICommandHandlerMiddleware<TCommand>
        where TCommand : ICommand
    {
        Task<bool> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
