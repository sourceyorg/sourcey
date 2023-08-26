using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sourcey.Events.Streams;

namespace Sourcey.EntityFrameworkCore.Events.ChangeTracking;

internal sealed class NullableStreamIdValueComparer : ValueComparer<StreamId?>
{
    public NullableStreamIdValueComparer()
        : base((a, b) => IsEqual(a, b), t => HashCode(t), t => CreateSnapshot(t)) { }

    private static bool IsEqual(StreamId? left, StreamId? right)
    {
        if (left == null || right == null)
            return false;

        return left.Value.Equals(right.Value);
    }

    private static int HashCode(StreamId? streamId)
    {
        if (streamId == null)
            return 0;

        if (streamId is IEquatable<StreamId>)
            return streamId.GetHashCode();

        return ConvertTo(streamId)?.GetHashCode() ?? 0;
    }

    private static StreamId? CreateSnapshot(StreamId? streamId)
    {
        return ConvertFrom(ConvertTo(streamId));
    }

    private static string? ConvertTo(StreamId? streamId)
    {
        if (streamId == null)
            return null;

        return (string)streamId;
    }

    private static StreamId? ConvertFrom(string? streamId)
    {
        if (string.IsNullOrWhiteSpace(streamId))
            return null;

        return StreamId.From(streamId);
    }
}
