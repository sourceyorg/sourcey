using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace EntityFrameworkCore.Projections.DbContexts;

public sealed class ReadonlySomethingContext(DbContextOptions<ReadonlySomethingContext> options) : SomethingContext(options);
