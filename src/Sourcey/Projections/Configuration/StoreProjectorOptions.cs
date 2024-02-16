namespace Sourcey.Projections.Configuration;

public interface IStoreProjectorOptions
{
    public IStoreProjectorOptions WithInterval(int interval);
    public IStoreProjectorOptions WithPageSize(int pageSize);
    public IStoreProjectorOptions WithRetries(int retryCount);
}

public sealed class StoreProjectorOptions : IStoreProjectorOptions
{
    public static string GetKey(string projection)
        => $"{nameof(StoreProjectorOptions)}_{projection}";
    
    public StoreProjectorOptions()
    {
    }

    internal int Interval { get; set; } = 5000;
    internal int PageSize { get; set; } = 500;
    internal int RetryCount { get; set; } = 5;

    public IStoreProjectorOptions WithInterval(int interval)
    {
        Interval = interval;
        return this;
    }

    public IStoreProjectorOptions WithPageSize(int pageSize)
    {
        PageSize = pageSize;
        return this;
    }

    public IStoreProjectorOptions WithRetries(int retryCount)
    {
        RetryCount = retryCount;
        return this;
    }
}
