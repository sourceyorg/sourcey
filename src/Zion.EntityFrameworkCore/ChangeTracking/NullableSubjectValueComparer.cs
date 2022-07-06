using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.ChangeTracking
{
    internal sealed class NullableSubjectValueComparer : ValueComparer<Subject?>
    {
        public NullableSubjectValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(Subject? left, Subject? right)
        {
            if (left == null || right == null)
                return false;

            return left.Value.Equals(right.Value);
        }

        private static int HashCode(Subject? subject)
        {
            if (subject == null)
                return 0;

            if (subject is IEquatable<Subject>)
                return subject.GetHashCode();

            return ConvertTo(subject)?.GetHashCode() ?? 0;
        }

        private static Subject? CreateSnapshot(Subject? subject)
        {
            return ConvertFrom(ConvertTo(subject));
        }

        private static string? ConvertTo(Subject? subject)
        {
            if (subject == null)
                return null;

            return (string)subject;
        }

        private static Subject? ConvertFrom(string? subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
                return null;

            return Subject.From(subject);
        }
    }
}
