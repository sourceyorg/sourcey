using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Queries.Execution;

namespace Sourcey.Queries.Builder
{
    internal readonly struct QueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IServiceCollection _services;

        public QueryBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IQueryBuilder<TQuery, TResult> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPostQueryMiddleware<TQuery, TResult>>(sp => new PostQueryHandler<TQuery, TResult>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IQueryBuilder<TQuery, TResult> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPreQueryMiddleware<TQuery, TResult>>(sp => new PreQueryHandler<TQuery, TResult>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IQueryBuilder<TQuery, TResult> WithQueryStoreLogging<TQueryStoreContext>()
        {
            _services.AddScoped<IPostQueryMiddleware<TQuery, TResult>, QueryStoreMiddleware<TQuery, TResult, TQueryStoreContext>>();
            return this;
        }

        public IQueryBuilder<TQuery, TResult> WithHandler<THandler>()
            where THandler : class, IQueryHandler<TQuery, TResult>
        {
            _services.TryAddScoped<IQueryHandler<TQuery, TResult>, THandler>();
            return this;
        }
    }
}
