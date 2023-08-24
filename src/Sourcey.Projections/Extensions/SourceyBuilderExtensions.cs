using Sourcey.Core.Builder;
using Sourcey.Projections;
using Sourcey.Projections.Builder;

namespace Sourcey.Extensions
{
    public static class SourceyBuilderExtensions
    {
        public static ISourceyBuilder AddProjection<TProjection>(this ISourceyBuilder source, Action<IProjectionBuilder<TProjection>> configure)
            where TProjection : class, IProjection
        {
            var builder = new ProjectionBuilder<TProjection>(source.Services);
            configure(builder);
            return source;
        }
    }
}
