namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;

public record class ProjectionStateDbType : DbType
{
    public ProjectionStateDbType(DbType original) : base(original)
    {
    }

    public ProjectionStateDbType(Type projectionType, Type optionsType, Type contextType) : base(projectionType, optionsType, contextType)
    {
    }
}
