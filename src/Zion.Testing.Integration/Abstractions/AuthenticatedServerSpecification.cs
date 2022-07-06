using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Xunit.Abstractions;
using Zion.Testing.Abstractions;
using Zion.Testing.Fixtures;
using Zion.Testing.Integration.Extensions;

namespace Zion.Testing.Integration.Abstractions
{
    public abstract class AuthenticatedServerSpecification<TStartup, TConfigurationFixture> : SpecificationWithConfiguration<TConfigurationFixture>, IClassFixture<ZionWebApplicationFactory<TStartup>>
        where TStartup : class
        where TConfigurationFixture : BaseConfigurationFixture
    {
        protected readonly ZionWebApplicationFactory<TStartup> _webApplicationFactory;

        protected abstract IEnumerable<Claim> Claims();

        protected virtual void BuildHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                foreach (var service in _services)
                    s.Add(service);
            });

            builder.WithAuthentication(new ClaimsProvider(Claims().ToList()));
        }

        protected override IServiceProvider BuildServiceProvider()
            => _webApplicationFactory.Services;

        public AuthenticatedServerSpecification(TConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper,
            ZionWebApplicationFactory<TStartup> webApplicationFactory)
            : base(configurationFixture, testOutputHelper)
        {
            _webApplicationFactory = webApplicationFactory;
            _webApplicationFactory.WithWebHostBuilder(BuildHost);
        }
    }

    public abstract class AuthenticatedServerSpecification<TStartup, TResult, TConfigurationFixture> : SpecificationWithConfiguration<TConfigurationFixture, TResult>, IClassFixture<ZionWebApplicationFactory<TStartup>>
        where TStartup : class
        where TConfigurationFixture : BaseConfigurationFixture
    {
        protected readonly ZionWebApplicationFactory<TStartup> _webApplicationFactory;

        protected abstract IEnumerable<Claim> Claims();

        protected virtual void BuildHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                foreach (var service in _services)
                    s.Add(service);
            });

            builder.WithAuthentication(new ClaimsProvider(Claims().ToList()));
        }

        protected override IServiceProvider BuildServiceProvider()
            => _webApplicationFactory.Services;

        public AuthenticatedServerSpecification(TConfigurationFixture configurationFixture,
            ITestOutputHelper testOutputHelper,
            ZionWebApplicationFactory<TStartup> webApplicationFactory)
            : base(configurationFixture, testOutputHelper)
        {
            _webApplicationFactory = webApplicationFactory;
            _webApplicationFactory.WithWebHostBuilder(BuildHost);
        }
    }
}
