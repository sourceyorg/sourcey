namespace Zion.Azure.Encryption
{
    public class KeyVaultOptions
    {
        public string Name { get; set; }
        public string TennatId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string EncryptionKey { get; set; }
    }
}
