using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Entities
{
    public class ProjectionState : IProjectionState
    {
        public string Key { get; set; }
        public long Position { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string? Error { get; set; }
        public string? ErrorStackTrace { get; set; }

    }
}
