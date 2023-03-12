using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Zion.Core.Builder;
using Zion.Queries;
using Zion.Queries.Builder;
using Zion.Queries.Cache;
using Zion.Queries.Execution;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddQuery<TQuery, TResult>(this IZionBuilder builder, Action<IZionQueryBuilder<TQuery, TResult>> configuration)
            where TQuery : IQuery<TResult>
        {
            builder.Services.TryAddScoped<IQueryDispatcher, QueryDispatcher>();

            var zionQueryBuilder = new ZionQueryBuilder<TQuery, TResult>(builder.Services);
            configuration(zionQueryBuilder);

            return builder;
        }

        public static IZionBuilder RegisterQueryCache<TQuery, TResult>(this IZionBuilder builder)
            where TQuery : IQuery<TResult>
        {
            builder.Services.TryAddSingleton<IQueryTypeCache, QueryTypeCache>();
            builder.Services.AddSingleton(new QueryTypeCacheRecord(typeof(TQuery)));
            return builder;
        }

        public static IZionBuilder RegisterQueryCache(this IZionBuilder builder, params Type[] types)
        {
            builder.Services.TryAddSingleton<IQueryTypeCache, QueryTypeCache>();

            foreach (var type in types)
                builder.Services.AddSingleton(new QueryTypeCacheRecord(type));

            return builder;
        }

        public static IZionBuilder RegisterQueryCache(this IZionBuilder builder)
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
