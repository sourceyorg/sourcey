namespace Sourcey.Projections.Configuration;

public sealed class StoreProjectorOptions<TProjection>
    where TProjection : class, IProjection
{
    public StoreProjectorOptions()
    {

    }

    public int Interval { get; set; }
    public int PageSize { get; set; }
    public int RetryCount { get; set; }


    public static StoreProjectorOptions<TProjection> Default = new()
    {
        Interval = 5000,
        PageSize = 500,
        RetryCount = 5
    };
}
