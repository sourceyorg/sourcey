using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.Factories.DbContexts
{
    public interface IDbTypeFactory<TDbType>
        where TDbType : DbType
    {
        TDbType Create<TProjection>()
            where TProjection : class, IProjection;
    }
}
