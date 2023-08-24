namespace Sourcey.Encryption.Aes
{
    public sealed class AesOptions
    {
        public byte[]? Key { get; set; }
        public byte[]? IV { get; set; }
    }
}
