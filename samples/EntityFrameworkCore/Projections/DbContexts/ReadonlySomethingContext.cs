using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Projections.DbContexts;

public sealed class ReadonlySomethingContext(DbContextOptions<ReadonlySomethingContext> options) : SomethingContext(options);
