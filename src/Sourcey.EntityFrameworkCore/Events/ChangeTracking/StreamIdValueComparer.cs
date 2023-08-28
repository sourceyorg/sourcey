using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Keys;

namespace Sourcey.EntityFrameworkCore.Events.ChangeTracking;

internal sealed class StreamIdValueComparer : ValueComparer<StreamId>
{
    public StreamIdValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(StreamId left, StreamId right)
        => left.Equals(right);
    private static int HashCode(StreamId streamId)
        => streamId.GetHashCode();
    private static StreamId CreateSnapshot(StreamId streamId)
        => ConvertFrom(ConvertTo(streamId));
    private static string ConvertTo(StreamId streamId)
        => (string)streamId;
    private static StreamId ConvertFrom(string streamId)
        => StreamId.From(streamId);
}
