using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections;

public sealed class ReadonlySomethingContext(DbContextOptions<ReadonlySomethingContext> options) : SomethingContext(options);
