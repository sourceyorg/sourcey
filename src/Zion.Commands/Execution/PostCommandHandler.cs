namespace Zion.Commands.Execution
{
    internal sealed class PostCommandHandler<TCommand> : IPostCommandMiddleware<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandlerMiddleware<TCommand> _middleware;
        public PostCommandHandler(ICommandHandlerMiddleware<TCommand> middleware)
        {
            if (middleware is null)
                throw new ArgumentNullException(nameof(middleware));
            
            _middleware = middleware;
        }
        public Task<bool> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
            => _middleware.ExecuteAsync(command, cancellationToken);
    }
}
