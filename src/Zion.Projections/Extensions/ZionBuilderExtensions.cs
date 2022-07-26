using Zion.Core.Builder;
using Zion.Projections;
using Zion.Projections.Builder;

namespace Zion.Extensions
{
    public static class ZionBuilderExtensions
    {
        public static IZionBuilder AddProjection<TProjection>(this IZionBuilder source, Action<IZionProjectionBuilder<TProjection>> configure)
            where TProjection : class, IProjection
        {
            var builder = new ZionProjectionBuilder<TProjection>(source.Services);
            configure(builder);
            return source;
        }
    }
}
