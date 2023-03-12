using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Zion.Commands;
using Zion.Commands.Builder;
using Zion.Commands.Cache;
using Zion.Commands.Execution;
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

        public static IZionBuilder RegisterCommandCache<TCommand>(this IZionBuilder builder)
            where TCommand : ICommand
        {
            builder.Services.TryAddSingleton<ICommandTypeCache, CommandTypeCache>();
            builder.Services.AddSingleton(new CommandTypeCacheRecord(typeof(TCommand)));
            return builder;
        }

        public static IZionBuilder RegisterCommandCache(this IZionBuilder builder, params Type[] types)
        {
            builder.Services.TryAddSingleton<ICommandTypeCache, CommandTypeCache>();

            foreach (var type in types)
                builder.Services.AddSingleton(new CommandTypeCacheRecord(type));

            return builder;
        }

        public static IZionBuilder RegisterCommandCache(this IZionBuilder builder)
        {
            var commandType = typeof(ICommand);

            var assemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToArray();

            var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                                  .Where(typeInfo => typeInfo.IsClass && !typeInfo.IsAbstract)
                                  .Where(typeInfo => commandType.IsAssignableFrom(typeInfo))
                                  .Select(typeInfo => typeInfo.AsType())
                                  .ToArray();

            return builder.RegisterCommandCache(types);
        }
    }
}
