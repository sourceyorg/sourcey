using Zion.EntityFrameworkCore.Commands.DbContexts;

namespace Zion.EntityFrameworkCore.Commands.Factories
{
    public interface ICommandStoreDbContextFactory
    {
        CommandStoreDbContext Create();
    }
}
