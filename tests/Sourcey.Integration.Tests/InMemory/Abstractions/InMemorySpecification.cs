using Xunit.Abstractions;

namespace Sourcey.Integration.Tests.InMemory;

public class Path
{
    public string[] Relays { get; set; }
    public string Mailbox { get; set; }
    public string Domain { get; set; }
    public string Params { get; set; }
}

public record Message(string Id)
{
    public string ID { get; set; }
    public Path From { get; set; }
    public Path[] To { get; set; }
    public DateTime Created { get; set; }
}

public record Messages(int Total, int Count, int Start, Message[] Items);


public abstract class InMemorySpecification : Specification
{
    protected readonly InMemoryWebApplicationFactory _factory;

    protected InMemorySpecification(
        InMemoryWebApplicationFactory factory,
        ITestOutputHelper testOutputHelper ) : base(testOutputHelper)
    {
        _factory = factory;
    }
}
