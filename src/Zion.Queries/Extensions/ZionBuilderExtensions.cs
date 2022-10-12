using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Core.Builder;
using Zion.Queries;
using Zion.Queries.Builder;
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
    }
}
