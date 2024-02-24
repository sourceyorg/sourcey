namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Readonly;

internal record ReadonlyProjectionDbType : DbType
{
    public ReadonlyProjectionDbType(DbType original) : base(original)
    {
    }

    public ReadonlyProjectionDbType(Type projectionType, Type optionsType, Type contextType) : base(projectionType, optionsType, contextType)
    {
    }
}
