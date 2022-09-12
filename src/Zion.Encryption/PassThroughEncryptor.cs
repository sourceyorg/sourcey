using System.Text;

namespace Zion.Encryption
{
    internal class PassThroughEncryptor : IEncryptor
    {
        public Task<string> DecryptAsync(Secret data, CancellationToken cancellationToken = default)
            => Task.FromResult(Encoding.UTF8.GetString(data));

        public Task<Secret> EncryptAsync(ReadOnlyMemory<char> data, CancellationToken cancellationToken = default)
            => Task.FromResult(Secret.From(Encoding.UTF8.GetBytes(data.ToArray())));
    }
}
