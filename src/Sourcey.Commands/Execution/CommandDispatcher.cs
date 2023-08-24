using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sourcey.Core.Extensions;

namespace Sourcey.Commands.Execution
{
    internal sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly ILogger<CommandDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(ILogger<CommandDispatcher> logger,
                                 IServiceProvider serviceProvider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation($"Dispatching command '{typeof(TCommand).FriendlyName()}'.");

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
                throw new InvalidOperationException($"No command handler for type '{typeof(TCommand).FriendlyName()}' has been registered.");
            
            if (!await DispatchMiddleWare<IPreCommandMiddleware<TCommand>, TCommand>(command, cancellationToken))
                return;
            
            await handler.ExecuteAsync(command, cancellationToken);

            await DispatchMiddleWare<IPostCommandMiddleware<TCommand>, TCommand>(command, cancellationToken);
        }

        private async Task<bool> DispatchMiddleWare<TMiddleware, TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
            where TMiddleware : ICommandHandlerMiddleware<TCommand>
        {
            var middlewares = _serviceProvider.GetServices<TMiddleware>();

            if (middlewares is null)
                return true;

            foreach (var middleware in middlewares)
                if (!await middleware.ExecuteAsync(command, cancellationToken))
                    return false;

            return true;
        }
    }
}
