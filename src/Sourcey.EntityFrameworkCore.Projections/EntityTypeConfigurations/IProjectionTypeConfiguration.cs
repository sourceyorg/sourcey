using Microsoft.EntityFrameworkCore;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.EntityTypeConfigurations
{
    public interface IProjectionTypeConfiguration<TProjection> : IEntityTypeConfiguration<TProjection>
        where TProjection : class, IProjection
    {
    }
}
