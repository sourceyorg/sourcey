using Microsoft.EntityFrameworkCore;
using Zion.Projections;

namespace Zion.EntityFrameworkCore.Projections.EntityTypeConfigurations
{
    public interface IProjectionTypeConfiguration<TProjection> : IEntityTypeConfiguration<TProjection>
        where TProjection : class, IProjection
    {
    }
}
