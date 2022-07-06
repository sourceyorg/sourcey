using Zion.Core.Extensions;

namespace Zion.EntityFrameworkCore.Projections.Factories
{
    internal sealed class ProjectionDbContextTypesFactory : IProjectionDbContextTypesFactory
    {
        private readonly IDictionary<string, ProjectionDbTypes> _projectionDbTypes;

        public ProjectionDbContextTypesFactory(IEnumerable<ProjectionDbTypes> projectionDbTypes)
        {
            _projectionDbTypes = projectionDbTypes?
                .DistinctBy(pdbt => pdbt.ProjectionType.FriendlyName())
                ?.ToDictionary(pdbt => pdbt.ProjectionType.FriendlyName(), pdbt => pdbt)
                ?? new Dictionary<string, ProjectionDbTypes>();

        }

        ProjectionDbTypes IProjectionDbContextTypesFactory.Create<TProjection>()
        {
            var projectionName = typeof(TProjection).FriendlyFullName();

            if (!_projectionDbTypes.TryGetValue(projectionName, out var types) || types is null)
            {
                throw new InvalidOperationException($"No db context registered against projection: {projectionName}");
            }

            return types;
        }
    }
}
