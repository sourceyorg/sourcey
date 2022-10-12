using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands.Execution;

namespace Zion.Commands.Builder
{
    internal readonly struct ZionCommandBuilder<TCommand> : IZionCommandBuilder<TCommand>
        where TCommand : ICommand
    {
        private readonly IServiceCollection _services;

        public ZionCommandBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }
        
        public IZionCommandBuilder<TCommand> AfterHandler<TMiddleWare>() 
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPostCommandMiddleware<TCommand>>(sp => new PostCommandHandler<TCommand>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IZionCommandBuilder<TCommand> BeforeHandler<TMiddleWare>() 
            where TMiddleWare : class, ICommandHandlerMiddleware<TCommand>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPreCommandMiddleware<TCommand>>(sp => new PreCommandHandler<TCommand>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IZionCommandBuilder<TCommand> WithCommandStoreLogging<TCommandStoreContext>()
        {
            _services.AddScoped<IPostCommandMiddleware<TCommand>, CommandStoreMiddleware<TCommand, TCommandStoreContext>>();
            return this;
        }

        public IZionCommandBuilder<TCommand> WithHandler<THandler>() 
            where THandler : class, ICommandHandler<TCommand>
        {
            _services.TryAddScoped<ICommandHandler<TCommand>, THandler>();
            return this;
        }
    }
}
