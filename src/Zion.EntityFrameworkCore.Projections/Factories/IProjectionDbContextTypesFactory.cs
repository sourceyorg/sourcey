using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Factories
{
    internal interface IProjectionDbContextTypesFactory
    {
        ProjectionDbTypes Create<TProjection>()
            where TProjection : class, IProjection;
    }
}
