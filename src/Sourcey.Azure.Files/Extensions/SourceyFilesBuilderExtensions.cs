using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sourcey.Files;
using Sourcey.Files.Builder;

namespace Sourcey.Azure.Files.Extensions
{
    public static class SourceyFilesBuilderExtensions
    {
        public static IFilesBuilder WithAzureStorage(this IFilesBuilder builder, Action<FileClientOptions> action)
        {
            var options = new FileClientOptions();
            action(options);

            builder.Services.TryAddScoped<IFileClient, FileClient>();

            builder.Services.AddAzureClients(clientBuilder 
                => clientBuilder.AddBlobServiceClient(options.ConnectionString)
                    .WithName(options.Name));

            return builder;
        }
    }
}
