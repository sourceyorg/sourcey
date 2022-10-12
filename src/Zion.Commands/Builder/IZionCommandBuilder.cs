using Zion.Commands.Execution;

namespace Zion.Commands.Builder
{
    public interface IZionCommandBuilder<TCommand>
        where TCommand : ICommand
    {
        IZionCommandBuilder<TCommand> WithHandler<THandler>()
            where THandler : class, ICommandHandler<TCommand>;
        IZionCommandBuilder<TCommand> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>;
        IZionCommandBuilder<TCommand> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>;
        IZionCommandBuilder<TCommand> WithCommandStoreLogging<TCommandStoreContext>();
    }
}
