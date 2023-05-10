namespace HaWeb.XMLTests;

public interface INodeRule {
    public string Name { get; }
    public string XPath { get; }
    public string? uniquenessAttribute { get; }
    public List<(string LinkAttribute, string RemoteElement, string RemoteAttribute)>? References { get; }
    public string[]? Attributes { get; }
}