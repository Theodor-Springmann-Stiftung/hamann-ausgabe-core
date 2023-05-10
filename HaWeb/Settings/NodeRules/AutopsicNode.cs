namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class AutopsicNode : INodeRule
{
    public string Name => "autopsic";
    public string XPath => "//autopsic";
    public string[]? Attributes { get; } = { "value" };
    public string? uniquenessAttribute => "value" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}