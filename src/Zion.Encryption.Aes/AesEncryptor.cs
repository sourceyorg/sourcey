using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Zion.Encryption.Aes
{
    internal sealed class AesEncryptor : IEncryptor
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptor(IOptionsSnapshot<AesOptions> options)
        {
            if(options == null)
                throw new ArgumentNullException(nameof(options));
            if (options.Value?.Key == null)
                throw new ArgumentNullException(nameof(options.Value.Key));
            if (options.Value.IV == null)
                throw new ArgumentNullException(nameof(options.Value.IV));

            _key = options.Value.Key;
            _iv = options.Value.IV;
        }

        public async Task<string> DecryptAsync(Secret secret, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var msDecrypt = new MemoryStream(secret);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return await srDecrypt.ReadToEndAsync();
        }

        public async Task<Secret> EncryptAsync(ReadOnlyMemory<char> data, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            using var sr = new StreamReader(cs);

            await sw.WriteAsync(data, cancellationToken);

            return Secret.From(ms.ToArray());
        }
    }
}
