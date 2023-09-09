namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class AutopsicNode : INodeRule
{
    public string Name => "autopsic";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "metadaten" },
        XPath = "//autopsic"
    };
    public string[]? Attributes { get; } = { "value" };
    public string? uniquenessAttribute => "value" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}