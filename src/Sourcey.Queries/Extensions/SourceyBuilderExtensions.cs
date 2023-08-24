using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Sourcey.Core.Builder;
using Sourcey.Queries;
using Sourcey.Queries.Builder;
using Sourcey.Queries.Cache;
using Sourcey.Queries.Execution;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddQuery<TQuery, TResult>(this ISourceyBuilder builder, Action<IQueryBuilder<TQuery, TResult>> configuration)
            where TQuery : IQuery<TResult>
        {
            builder.Services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();

            var sourceyQueryBuilder = new QueryBuilder<TQuery, TResult>(builder.Services);
            configuration(sourceyQueryBuilder);

            return builder;
        }

        public static ISourceyBuilder RegisterQueryCache<TQuery, TResult>(this ISourceyBuilder builder)
            where TQuery : IQuery<TResult>
        {
            builder.Services.TryAddSingleton<IQueryTypeCache, QueryTypeCache>();
            builder.Services.AddSingleton(new QueryTypeCacheRecord(typeof(TQuery)));
            return builder;
        }

        public static ISourceyBuilder RegisterQueryCache(this ISourceyBuilder builder, params Type[] types)
        {
            builder.Services.TryAddSingleton<IQueryTypeCache, QueryTypeCache>();

            foreach (var type in types)
                builder.Services.AddSingleton(new QueryTypeCacheRecord(type));

            return builder;
        }

        public static ISourceyBuilder RegisterQueryCache(this ISourceyBuilder builder)
        {
            var queryType = typeof(IQuery<>);

            var assemblies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToArray();

            var types = assemblies.SelectMany(assembly => assembly.DefinedTypes)
                                  .Where(t => !t.IsAbstract)
                                  .Select(t => new
                                  {
                                      type = t,
                                      interfaces = t.GetInterfaces(),
                                      baseType = t.BaseType
                                  })
                                  .Where(t =>
                                    (t.baseType != null
                                        && t.baseType.IsGenericType
                                        && queryType.IsAssignableFrom(t.baseType.GetGenericTypeDefinition()))
                                    || (t.interfaces.Any(i => i.IsGenericType && queryType.IsAssignableFrom(i.GetGenericTypeDefinition()))))
                                  .Select(typeInfo => typeInfo.type.AsType())
                                  .ToArray();

            return builder.RegisterQueryCache(types);
        }
    }
}
