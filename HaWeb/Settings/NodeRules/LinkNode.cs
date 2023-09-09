namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LinkNode : INodeRule
{
    public string Name => "link";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "ueberlieferung", "stellenkommentar", "register", "texteingriffe" },
        XPath = "//link"
    };
    public string[]? Attributes { get; } = null;
    public string? uniquenessAttribute => null;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
        ("ref", new HamannXPath() { Documents = new[] { "register" }, XPath = "//kommentar" }, "id"),
        ("subref", new HamannXPath() { Documents = new[] { "register" }, XPath = "//subsection" }, "id"),
    };
}