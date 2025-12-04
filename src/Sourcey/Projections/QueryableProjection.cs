using System.Collections;
using System.Linq.Expressions;

namespace Sourcey.Projections;
public sealed class QueryableProjection<TProjection>(
    IQueryable<TProjection> queryable,
    IAsyncDisposable asyncDisposable)
    : IQueryableProjection<TProjection>
    where TProjection : class, IProjection
{
    public Type ElementType => queryable.ElementType;

    public Expression Expression => queryable.Expression;

    public IQueryProvider Provider => queryable.Provider;

    public ValueTask DisposeAsync() => asyncDisposable.DisposeAsync();

    public IEnumerator<TProjection> GetEnumerator() => queryable.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
