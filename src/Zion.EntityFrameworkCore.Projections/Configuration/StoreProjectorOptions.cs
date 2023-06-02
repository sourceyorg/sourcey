using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Configuration
{
    internal sealed class StoreProjectorOptions<TProjection>
        where TProjection : class, IProjection
    {
        public StoreProjectorOptions()
        {

        }

        public int Interval { get; set; }
        public int PageSize { get; set; }
        public int RetryCount { get; set; }


        internal static StoreProjectorOptions<TProjection> Default = new()
        {
            Interval = 5000,
            PageSize = 500,
            RetryCount = 5
        };
    }
}
