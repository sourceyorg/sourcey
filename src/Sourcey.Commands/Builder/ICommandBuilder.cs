using Sourcey.Commands.Execution;

namespace Sourcey.Commands.Builder
{
    public interface ICommandBuilder<TCommand>
        where TCommand : ICommand
    {
        ICommandBuilder<TCommand> WithHandler<THandler>()
            where THandler : class, ICommandHandler<TCommand>;
        ICommandBuilder<TCommand> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>;
        ICommandBuilder<TCommand> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>;
        ICommandBuilder<TCommand> WithCommandStoreLogging<TCommandStoreContext>();
    }
}
