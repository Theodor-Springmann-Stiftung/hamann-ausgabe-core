namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class AppNode : INodeRule
{
    public string Name => "app";
    public string XPath => "//app";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//appDef", "index")
    };
}