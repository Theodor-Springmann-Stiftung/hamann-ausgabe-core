namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class SenderNode : INodeRule
{
    public string Name => "sender";
    public string XPath => "//sender";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//personDef", "index")
    };
}