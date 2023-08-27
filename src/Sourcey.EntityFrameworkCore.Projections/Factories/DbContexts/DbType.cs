namespace Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts;

public abstract record DbType(Type ProjectionType, Type OptionsType, Type ContextType);
