using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Zion.Core.Exceptions;
using Zion.Files;
using FileOptions = Zion.Files.FileOptions;

namespace Zion.Azure.Files
{
    internal sealed class FileClient : IFileClient
    {
        private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactory;
        private readonly IExceptionStream _exceptionStream;

        public FileClient(IAzureClientFactory<BlobServiceClient> azureClientFactory, IExceptionStream exceptionStream)
        {
            if (azureClientFactory is null)
                throw new ArgumentNullException(nameof(azureClientFactory));
            if (exceptionStream is null)
                throw new ArgumentNullException(nameof(exceptionStream));

            _azureClientFactory = azureClientFactory;
            _exceptionStream = exceptionStream;
        }

        public async ValueTask<Stream?> DownloadAsync(FileOptions options, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return null;
            }

            var container = client.GetBlobContainerClient(options.Folder);

            if (container is null || !await container.ExistsAsync(cancellationToken))
            {
                _exceptionStream.AddException(new UnableToFindContainerException($"Unable to find conatiner for: {options.Folder}"), cancellationToken);
                return null;
            }

            var blob = container.GetBlobClient(options.File);

            if(blob is null || !await blob.ExistsAsync(cancellationToken))
            {
                _exceptionStream.AddException(new UnableToFindBlobException($"Unable to find blob for: {options.File}"), cancellationToken);
                return null;
            }

            return await blob.OpenReadAsync(cancellationToken: cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, Stream data, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return;
            }

            var container = client.GetBlobContainerClient(options.Folder);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(options.File);
            await blob.UploadAsync(data, cancellationToken: cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, byte[] data, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return;
            }

            var container = client.GetBlobContainerClient(options.Folder);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(options.File);
            using var stream = await blob.OpenWriteAsync(true, cancellationToken: cancellationToken);
            await stream.WriteAsync(data, cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, Memory<byte> data, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return;
            }

            var container = client.GetBlobContainerClient(options.Folder);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(options.File);
            using var stream = await blob.OpenWriteAsync(true, cancellationToken: cancellationToken);
            await stream.WriteAsync(data, cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, string data, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return;
            }

            var container = client.GetBlobContainerClient(options.Folder);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(options.File);
            using var stream = await blob.OpenWriteAsync(true, cancellationToken: cancellationToken);
            await stream.WriteAsync(Encoding.Unicode.GetBytes(data), cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, Memory<char> data, CancellationToken cancellationToken = default)
        {
            var client = _azureClientFactory.CreateClient(options.Root);

            if (client is null)
            {
                _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                return;
            }

            var container = client.GetBlobContainerClient(options.Folder);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blob = container.GetBlobClient(options.File);
            using var stream = await blob.OpenWriteAsync(true, cancellationToken: cancellationToken);
            await stream.WriteAsync(Encoding.Unicode.GetBytes(data.ToArray()), cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
    }
}
