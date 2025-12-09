using Microsoft.EntityFrameworkCore;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections;

public sealed class ReadonlySomethingContext(DbContextOptions<ReadonlySomethingContext> options) : SomethingContext(options);
