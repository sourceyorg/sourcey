using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zion.Files;
using Zion.Files.Builder;

namespace Zion.Azure.Files.Extensions
{
    public static class ZionFilesBuilderExtensions
    {
        public static IZionFilesBuilder WithAzureStorage(this IZionFilesBuilder builder, Action<FileClientOptions> action)
        {
            var options = new FileClientOptions();
            action(options);

            builder.Services.TryAddSingleton<IFileClient, FileClient>();

            builder.Services.AddAzureClients(clientBuilder 
                => clientBuilder.AddBlobServiceClient(options.ConnectionString)
                    .WithName(options.Name));

            return builder;
        }
    }
}
