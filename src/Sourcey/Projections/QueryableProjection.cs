using System.Collections;
using System.Linq.Expressions;

namespace Sourcey.Projections;
public sealed class QueryableProjection<TProjection> : IQueryableProjection<TProjection>
    where TProjection : class, IProjection
{
    private readonly IQueryable<TProjection> _queryable;
    private readonly IAsyncDisposable _asyncDisposable;

    public QueryableProjection(IQueryable<TProjection> queryable, IAsyncDisposable asyncDisposable)
    {
        _queryable = queryable;
        _asyncDisposable = asyncDisposable;
    }

    public Type ElementType => _queryable.ElementType;

    public Expression Expression => _queryable.Expression;

    public IQueryProvider Provider => _queryable.Provider;

    public ValueTask DisposeAsync() => _asyncDisposable.DisposeAsync();

    public IEnumerator<TProjection> GetEnumerator() => _queryable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
