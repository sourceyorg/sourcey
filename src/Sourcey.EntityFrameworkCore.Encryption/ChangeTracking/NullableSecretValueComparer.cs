using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Encryption;

namespace Sourcey.EntityFrameworkCore.Encryption.ChangeTracking
{
    internal sealed class NullableSecretValueComparer : ValueComparer<Secret?>
    {
        public NullableSecretValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(Secret? left, Secret? right)
        {
            if (left == null || right == null)
                return false;

            return left.Value.Equals(right.Value);
        }

        private static int HashCode(Secret? secret)
        {
            if (secret == null)
                return 0;

            if (secret is IEquatable<Secret>)
                return secret.GetHashCode();

            return ConvertTo(secret)?.GetHashCode() ?? 0;
        }

        private static Secret? CreateSnapshot(Secret? secret)
        {
            return ConvertFrom(ConvertTo(secret));
        }

        private static byte[]? ConvertTo(Secret? secret)
        {
            if (secret == null)
                return null;

            return (byte[])secret;
        }

        private static Secret? ConvertFrom(byte[]? secret)
        {
            if (secret is null)
                return null;

            return Secret.From(secret);
        }
    }
}
