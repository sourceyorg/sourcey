namespace Sourcey.Files
{
    public interface IFileClient
    {
        public ValueTask<(Stream stream, string contentType)?> DownloadAsync(FileOptions options, CancellationToken cancellationToken = default);
        public ValueTask DeleteAsync(FileOptions options, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Stream data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, byte[] data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Memory<byte> data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, string data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Memory<char> data, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);
    }
}
