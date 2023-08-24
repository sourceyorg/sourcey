using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Commands.Execution;

namespace Sourcey.Commands.Builder
{
    internal readonly struct CommandBuilder<TCommand> : ICommandBuilder<TCommand>
        where TCommand : ICommand
    {
        private readonly IServiceCollection _services;

        public CommandBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }
        
        public ICommandBuilder<TCommand> AfterHandler<TMiddleWare>() 
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPostCommandMiddleware<TCommand>>(sp => new PostCommandHandler<TCommand>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public ICommandBuilder<TCommand> BeforeHandler<TMiddleWare>() 
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPreCommandMiddleware<TCommand>>(sp => new PreCommandHandler<TCommand>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public ICommandBuilder<TCommand> WithCommandStoreLogging<TCommandStoreContext>()
        {
            _services.AddScoped<IPostCommandMiddleware<TCommand>, CommandStoreMiddleware<TCommand, TCommandStoreContext>>();
            return this;
        }

        public ICommandBuilder<TCommand> WithHandler<THandler>() 
            where THandler : class, ICommandHandler<TCommand>
        {
            _services.TryAddScoped<ICommandHandler<TCommand>, THandler>();
            return this;
        }
    }
}
