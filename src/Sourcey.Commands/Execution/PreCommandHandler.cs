namespace Sourcey.Commands.Execution
{
    internal sealed class PreCommandHandler<TCommand> : IPreCommandMiddleware<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandlerMiddleware<TCommand> _middleware;
        public PreCommandHandler(ICommandHandlerMiddleware<TCommand> middleware)
        {
            if (middleware is null)
                throw new ArgumentNullException(nameof(middleware));

            _middleware = middleware;
        }
        public Task<bool> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
            => _middleware.ExecuteAsync(command, cancellationToken);
    }
}
