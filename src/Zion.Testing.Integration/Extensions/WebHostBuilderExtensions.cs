using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Zion.Testing.Integration.Abstractions;

namespace Zion.Testing.Integration.Extensions
{
    internal static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder WithAuthentication(this IWebHostBuilder builder, ClaimsProvider claimsProvider)
        {
            return builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(AuthenticatedAuthHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, AuthenticatedAuthHandler>(AuthenticatedAuthHandler.AuthenticationScheme, op => { });

                services.AddScoped(_ => claimsProvider);
            });
        }

        public static IWebHostBuilder WithAuthentication(this IWebHostBuilder builder)
        {
            return builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(AuthenticatedAuthHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, UnauthenticatedAuthHandler>(AuthenticatedAuthHandler.AuthenticationScheme, op => { });
            });
        }

        public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticatedAuthHandler.AuthenticationScheme);

            return client;
        }
    }
}
