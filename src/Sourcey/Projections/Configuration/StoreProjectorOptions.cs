namespace Sourcey.Projections.Configuration;

public sealed class StoreProjectorOptions<TProjection>
    where TProjection : class, IProjection
{
    public StoreProjectorOptions()
    {

    }

    internal int Interval { get; set; } = 5000;
    internal int PageSize { get; set; } = 500;
    internal int RetryCount { get; set; } = 5;

    public StoreProjectorOptions<TProjection> WithInterval(int interval)
    {
        Interval = interval;
        return this;
    }

    public StoreProjectorOptions<TProjection> WithPageSize(int pageSize)
    {
        PageSize = pageSize;
        return this;
    }

    public StoreProjectorOptions<TProjection> WithRetries(int retryCount)
    {
        RetryCount = retryCount;
        return this;
    }
}
