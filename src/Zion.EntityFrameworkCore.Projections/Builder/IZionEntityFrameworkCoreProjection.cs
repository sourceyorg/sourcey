using Microsoft.EntityFrameworkCore;
using Zion.EntityFrameworkCore.Projections.DbContexts;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Builder
{
    public interface IZionEntityFrameworkCoreProjection<TProjection>
        where TProjection : class, IProjection
    {
        IZionEntityFrameworkCoreProjection<TProjection> WithInterval(int interval);
    }
}
