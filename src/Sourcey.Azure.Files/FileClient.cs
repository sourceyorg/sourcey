using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using Sourcey.Core.Exceptions;
using Sourcey.Files;
using FileOptions = Sourcey.Files.FileOptions;

namespace Sourcey.Azure.Files
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

        public async ValueTask DeleteAsync(FileOptions options, CancellationToken cancellationToken = default)
        {
            try
            {

                var client = _azureClientFactory.CreateClient(options.Root);

                if (client is null)
                {
                    _exceptionStream.AddException(new UnableToFindBlobServiceClientException($"No BlobServiceClient has been registered for: {options.Root}"), cancellationToken);
                    return;
                }

                var container = client.GetBlobContainerClient(options.Folder);

                if (container is null || !await container.ExistsAsync(cancellationToken))
                {
                    _exceptionStream.AddException(new UnableToFindContainerException($"Unable to find conatiner for: {options.Folder}"), cancellationToken);
                    return;
                }

                var blob = container.GetBlobClient(options.File);
                await blob.DeleteAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _exceptionStream.AddException(ex, cancellationToken);
            }
        }

        public async ValueTask<(Stream stream, string contentType)?> DownloadAsync(FileOptions options, CancellationToken cancellationToken = default)
        {
            try
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

                if (blob is null || !await blob.ExistsAsync(cancellationToken))
                {
                    _exceptionStream.AddException(new UnableToFindBlobException($"Unable to find blob for: {options.File}"), cancellationToken);
                    return null;
                }

                var stream = await blob.OpenReadAsync(cancellationToken: cancellationToken);
                var properties = await blob.GetPropertiesAsync(cancellationToken: cancellationToken);

                return (stream, string.IsNullOrWhiteSpace(properties.Value.ContentType) ? "application/octet-stream" : properties.Value.ContentType);
            }
            catch (Exception ex)
            {
                _exceptionStream.AddException(ex, cancellationToken);
                return null;
            }
        }

        public async ValueTask UploadAsync(FileOptions options, Stream data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            try
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
                await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
                await blob.UploadAsync(data, cancellationToken: cancellationToken);
                await blob.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _exceptionStream.AddException(ex, cancellationToken);
            }
        }

        public async ValueTask UploadAsync(FileOptions options, byte[] data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(data);
            await UploadAsync(options, ms, contentType, cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, Memory<byte> data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(data.ToArray());
            await UploadAsync(options, ms, contentType, cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, string data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(Encoding.Unicode.GetBytes(data));
            await UploadAsync(options, ms, contentType, cancellationToken);
        }

        public async ValueTask UploadAsync(FileOptions options, Memory<char> data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(Encoding.Unicode.GetBytes(data.ToArray()));
            await UploadAsync(options, ms, contentType, cancellationToken);
        }
    }
}
