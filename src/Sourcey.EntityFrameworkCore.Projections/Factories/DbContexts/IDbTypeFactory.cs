using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;

public interface IDbTypeFactory<TDbType>
    where TDbType : DbType
{
    TDbType Create<TProjection>()
        where TProjection : class, IProjection;
}
