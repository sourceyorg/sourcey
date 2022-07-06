using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, AuthenticatedAuthHandler>("Test", op => { });

                services.AddScoped(_ => claimsProvider);
            });
        }

        public static IWebHostBuilder WithAuthentication(this IWebHostBuilder builder)
        {
            return builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, UnauthenticatedAuthHandler>("Test", op => { });
            });
        }

        public static HttpClient CreateClientWithTestAuth<T>(this WebApplicationFactory<T> factory) where T : class
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            return client;
        }
    }
}
