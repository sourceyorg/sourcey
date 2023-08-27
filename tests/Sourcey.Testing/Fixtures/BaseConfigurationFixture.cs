﻿using Microsoft.Extensions.Configuration;

namespace Sourcey.Testing.Fixtures;

public class BaseConfigurationFixture
{
    public IConfiguration Configuration { get; }

    public BaseConfigurationFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddUserSecrets(GetType().Assembly, optional: true)
            .AddEnvironmentVariables(prefix: "Sourcey_")
            .Build();

        Configuration = configuration;
    }
}
