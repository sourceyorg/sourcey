using Microsoft.EntityFrameworkCore;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections;

public sealed class WriteableSomethingContext(DbContextOptions<WriteableSomethingContext> options) : SomethingContext(options);
