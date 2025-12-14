using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Projections.DbContexts;

public sealed class WriteableSomethingContext(DbContextOptions<WriteableSomethingContext> options) : SomethingContext(options);
