namespace HaWeb.XMLTests;

public interface INodeRule {
    public string Name { get; }
    public HamannXPath XPath { get; }
    public string? uniquenessAttribute { get; }
    public List<(string LinkAttribute, HamannXPath RemoteElement, string RemoteAttribute)>? References { get; }
    public string[]? Attributes { get; }
}