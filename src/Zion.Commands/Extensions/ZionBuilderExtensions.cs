using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Commands;
using Zion.Commands.Builder;
using Zion.Commands.Execution;
using Zion.Commands.Stores;
using Zion.Core.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddCommand<TCommand>(this IZionBuilder builder, Action<IZionCommandBuilder<TCommand>> configuration)
            where TCommand : ICommand
        {
            builder.Services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();

            var zionCommandBuilder = new ZionCommandBuilder<TCommand>(builder.Services);
            configuration(zionCommandBuilder);
            
            return builder;
        }
    }
}
