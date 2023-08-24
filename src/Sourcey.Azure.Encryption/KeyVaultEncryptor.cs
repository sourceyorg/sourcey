using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Options;
using Sourcey.Encryption;

namespace Sourcey.Azure.Encryption
{
    internal sealed class KeyVaultEncryptor : IEncryptor
    {
        private readonly KeyClient _keyClient;
        private readonly ClientSecretCredential _clientSecretCredential;
        private readonly string _encryptionKey;

        public KeyVaultEncryptor(IOptionsSnapshot<KeyVaultOptions> options)
        {
            _clientSecretCredential = new ClientSecretCredential(options.Value.TennatId, options.Value.ClientId, options.Value.ClientSecret);
            _keyClient = new KeyClient(new($"https://{options.Value.Name}.vault.azure.net/"), _clientSecretCredential);
            _encryptionKey = options.Value.EncryptionKey;
        }

        public async Task<string> DecryptAsync(Secret data, CancellationToken cancellationToken = default)
        {
            var key = await _keyClient.GetKeyAsync(_encryptionKey);
            var cryptoClient = new CryptographyClient(key.Value.Id, _clientSecretCredential);
            var dencryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep, data);
            return Encoding.UTF8.GetString(dencryptResult.Plaintext);
        }

        public async Task<Secret> EncryptAsync(ReadOnlyMemory<char> data, CancellationToken cancellationToken = default)
        {
            var key = await _keyClient.GetKeyAsync(_encryptionKey);
            var cryptoClient = new CryptographyClient(key.Value.Id, _clientSecretCredential);
            var byteData = Encoding.UTF8.GetBytes(data.ToArray());
            var encryptResult = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep, byteData);
            return Secret.From(encryptResult.Ciphertext);
        }
    }
}
