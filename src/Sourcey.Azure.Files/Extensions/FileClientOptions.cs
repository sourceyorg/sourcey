namespace Sourcey.Azure.Files.Extensions
{
    public class FileClientOptions
    {
        internal string Name { get; private set; }
        internal string ConnectionString { get; private set; }
        public void WithName(string name) => Name = name;
        public void WithConnectionString(string connectionString) => ConnectionString = connectionString;
    }
}
