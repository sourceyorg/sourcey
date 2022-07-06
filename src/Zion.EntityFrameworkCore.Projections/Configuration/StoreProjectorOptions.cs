using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Configuration
{
    public sealed class StoreProjectorOptions<TProjection>
        where TProjection : class, IProjection
    {
        public StoreProjectorOptions()
        {

        }

        public int Interval { get; set; }

        internal static Action<StoreProjectorOptions<TProjection>> Default => o => o.Interval = 5000;
    }
}
