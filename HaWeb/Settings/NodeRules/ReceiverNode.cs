namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class Receiver : INodeRule
{
    public string Name => "receiver";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "metadaten" },
        XPath = "//receiver"
    };
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
        ("ref", new HamannXPath() { Documents = new[] { "personenorte" }, XPath =  "//personDef" }, "index")
    };
}