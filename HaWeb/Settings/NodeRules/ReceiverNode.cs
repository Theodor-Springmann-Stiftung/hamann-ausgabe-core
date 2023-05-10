namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class Receiver : INodeRule
{
    public string Name => "receiver";
    public string XPath => "//receiver";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//personDef", "index")
    };
}