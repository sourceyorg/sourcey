using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Encryption;

namespace Zion.EntityFrameworkCore.Encryption.ChangeTracking
{
    internal sealed class SecretValueComparer : ValueComparer<Secret>
    {
        public SecretValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(Secret left, Secret right)
            => left.Equals(right);
        private static int HashCode(Secret secret)
            => secret.GetHashCode();
        private static Secret CreateSnapshot(Secret secret)
            => ConvertFrom(ConvertTo(secret));
        private static byte[] ConvertTo(Secret secret)
            => (byte[])secret;
        private static Secret ConvertFrom(byte[] secret)
            => Secret.From(secret);
    }
}
