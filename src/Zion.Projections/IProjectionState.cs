namespace Zion.Projections
{
    public interface IProjectionState
    {
        public string Key { get; set; }
        public long Position { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
