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
        

        internal static StoreProjectorOptions<TProjection> Default = new()
        {
            Interval = 5000
        };
    }
}
