namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class MarginalNode : INodeRule
{
    public string Name => "marginal";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "stellenkommentar" },
        XPath = "//marginal"
    };
    public string[]? Attributes { get; } = { "index", "letter", "page", "line" };
    public string? uniquenessAttribute => "index";
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}