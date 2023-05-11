namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LinkNode : INodeRule
{
    public string Name => "link";
    public string XPath => "//link";
    public string[]? Attributes { get; } = null;
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//kommentar", "id"),
        ("subref", "//subsection", "id")
    };
}