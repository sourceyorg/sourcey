namespace Sourcey.Encryption
{
    public interface IEncryptor
    {
        Task<Secret> EncryptAsync(ReadOnlyMemory<char> data, CancellationToken cancellationToken = default);
        Task<string> DecryptAsync(Secret data, CancellationToken cancellationToken = default);
    }
}
