using Microsoft.EntityFrameworkCore;
using Sourcey.EntityFrameworkCore.Projections.DbContexts;

namespace Sourcey.Integration.Tests.EntityFrameworkCore.Projections;

public sealed class WriteableSomethingContext(DbContextOptions<WriteableSomethingContext> options) : SomethingContext(options);
