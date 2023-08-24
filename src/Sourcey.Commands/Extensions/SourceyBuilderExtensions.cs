using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Sourcey.Commands;
using Sourcey.Commands.Builder;
using Sourcey.Commands.Cache;
using Sourcey.Commands.Execution;
using Sourcey.Core.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddCommand<TCommand>(this ISourceyBuilder builder, Action<ICommandBuilder<TCommand>> configuration)
            where TCommand : ICommand
        {
            builder.Services.TryAddScoped<ICommandDispatcher, CommandDispatcher>();

            var sourceyCommandBuilder = new CommandBuilder<TCommand>(builder.Services);
            configuration(sourceyCommandBuilder);
            
            return builder;
        }

        public static ISourceyBuilder RegisterCommandCache<TCommand>(this ISourceyBuilder builder)
            where TCommand : ICommand
        {
            builder.Services.TryAddSingleton<ICommandTypeCache, CommandTypeCache>();
            builder.Services.AddSingleton(new CommandTypeCacheRecord(typeof(TCommand)));
            return builder;
        }

        public static ISourceyBuilder RegisterCommandCache(this ISourceyBuilder builder, params Type[] types)
        {
            builder.Services.TryAddSingleton<ICommandTypeCache, CommandTypeCache>();

            foreach (var type in types)
                builder.Services.AddSingleton(new CommandTypeCacheRecord(type));

            return builder;
        }

        public static ISourceyBuilder RegisterCommandCache(this ISourceyBuilder builder)
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
