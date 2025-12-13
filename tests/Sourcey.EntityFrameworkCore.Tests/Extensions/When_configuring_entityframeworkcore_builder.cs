using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Sourcey.Extensions;

namespace Sourcey.EntityFrameworkCore.Tests.Extensions;

public class When_configuring_entityframeworkcore_builder
{
    [Then]
    public void AddEntityFrameworkCore_invokes_configuration_delegate()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddSourcey();
        var invoked = false;

        // Act
        var returned = builder.AddEntityFrameworkCore(_ => { invoked = true; });

        // Assert
        invoked.ShouldBeTrue();
        returned.ShouldBe(builder);
    }

    [Then]
    public void AddEntityFrameworkCoreMigrator_invokes_configuration_delegate()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = services.AddSourcey();
        var invoked = false;

        // Act
        var returned = builder.AddEntityFrameworkCoreMigrator(_ => { invoked = true; });

        // Assert
        invoked.ShouldBeTrue();
        returned.ShouldBe(builder);
    }
}
