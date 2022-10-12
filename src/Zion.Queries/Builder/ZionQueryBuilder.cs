using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Queries.Execution;

namespace Zion.Queries.Builder
{
    internal readonly struct ZionQueryBuilder<TQuery, TResult> : IZionQueryBuilder<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IServiceCollection _services;

        public ZionQueryBuilder(IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IZionQueryBuilder<TQuery, TResult> AfterHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPostQueryMiddleware<TQuery, TResult>>(sp => new PostQueryHandler<TQuery, TResult>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IZionQueryBuilder<TQuery, TResult> BeforeHandler<TMiddleWare>()
            where TMiddleWare : class, IQueryHandlerMiddleware<TQuery, TResult>
        {
            _services.AddScoped<TMiddleWare>();
            _services.AddScoped<IPreQueryMiddleware<TQuery, TResult>>(sp => new PreQueryHandler<TQuery, TResult>(sp.GetRequiredService<TMiddleWare>()));
            return this;
        }

        public IZionQueryBuilder<TQuery, TResult> WithQueryStoreLogging<TQueryStoreContext>()
        {
            _services.AddScoped<IPostQueryMiddleware<TQuery, TResult>, QueryStoreMiddleware<TQuery, TResult, TQueryStoreContext>>();
            return this;
        }

        public IZionQueryBuilder<TQuery, TResult> WithHandler<THandler>()
            where THandler : class, IQueryHandler<TQuery, TResult>
        {
            _services.TryAddScoped<IQueryHandler<TQuery, TResult>, THandler>();
            return this;
        }
    }
}
