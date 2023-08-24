namespace Sourcey.Queries.Execution
{
    internal class PostQueryHandler<TQuery, TResult> : IPostQueryMiddleware<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandlerMiddleware<TQuery, TResult> _middleware;
        public PostQueryHandler(IQueryHandlerMiddleware<TQuery, TResult> middleware)
        {
            if (middleware is null)
                throw new ArgumentNullException(nameof(middleware));

            _middleware = middleware;
        }
        public Task<bool> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default)
            => _middleware.ExecuteAsync(query, cancellationToken);
    }
}
