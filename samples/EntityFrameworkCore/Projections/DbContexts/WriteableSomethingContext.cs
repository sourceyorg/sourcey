using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace EntityFrameworkCore.Projections.DbContexts;

public sealed class WriteableSomethingContext(DbContextOptions<WriteableSomethingContext> options) : SomethingContext(options);
