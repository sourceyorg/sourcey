namespace Zion.Files
{
    public interface IFileClient
    {
        public ValueTask<Stream?> DownloadAsync(FileOptions options, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Stream data, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, byte[] data, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Memory<byte> data, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, string data, CancellationToken cancellationToken = default);
        public ValueTask UploadAsync(FileOptions options, Memory<char> data, CancellationToken cancellationToken = default);
    }
}
