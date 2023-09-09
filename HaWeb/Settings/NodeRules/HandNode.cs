namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class HandNode : INodeRule
{
    public string Name => "hand";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "ueberlieferung", "brieftext", "texteingriffe" },
        XPath = "//hand"
    };
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
        ("ref", new HamannXPath() { Documents = new[] { "personenorte" }, XPath =  "//handDef" }, "index")
    };
}