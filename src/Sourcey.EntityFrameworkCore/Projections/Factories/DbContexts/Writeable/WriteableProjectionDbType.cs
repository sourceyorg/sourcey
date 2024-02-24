namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;

internal record WriteableProjectionDbType : DbType
{
    public WriteableProjectionDbType(DbType original) : base(original)
    {
    }

    public WriteableProjectionDbType(Type projectionType, Type optionsType, Type contextType) : base(projectionType, optionsType, contextType)
    {
    }
}
