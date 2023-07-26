namespace Zion.Projections;
public interface IQueryableProjection<TProjection> : IQueryable<TProjection>, IAsyncDisposable
    where TProjection : class, IProjection
{
}
