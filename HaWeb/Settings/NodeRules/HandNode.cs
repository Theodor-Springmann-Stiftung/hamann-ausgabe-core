namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class HandNode : INodeRule
{
    public string Name => "hand";
    public string XPath => "//hand";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//handDef", "index")
    };
}