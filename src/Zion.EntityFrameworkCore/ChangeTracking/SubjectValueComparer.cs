using Microsoft.EntityFrameworkCore.ChangeTracking;
using Zion.Core.Keys;

namespace Zion.EntityFrameworkCore.ChangeTracking
{
    internal sealed class SubjectValueComparer : ValueComparer<Subject>
    {
        public SubjectValueComparer()
            : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

        private static bool IsEqual(Subject left, Subject right)
            => left.Equals(right);
        private static int HashCode(Subject subject)
            => subject.GetHashCode();
        private static Subject CreateSnapshot(Subject subject)
            => ConvertFrom(ConvertTo(subject));
        private static string ConvertTo(Subject subject)
            => (string)subject;
        private static Subject ConvertFrom(string subject)
            => Subject.From(subject);
    }
}
