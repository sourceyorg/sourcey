namespace Sourcey.Commands.Execution
{
    internal interface IPreCommandMiddleware<TCommand> : ICommandHandlerMiddleware<TCommand>
        where TCommand : ICommand
    {
    }
}
