using Zion.EntityFrameworkCore.Projections.Factories.DbContexts;

namespace Zion.EntityFrameworkCore.Projections.Factories.ProjecitonContexts
{
    internal record ProjectionDbType : DbType
    {
        public ProjectionDbType(DbType original) : base(original)
        {
        }

        public ProjectionDbType(Type projectionType, Type optionsType, Type contextType) : base(projectionType, optionsType, contextType)
        {
        }
    }
}
