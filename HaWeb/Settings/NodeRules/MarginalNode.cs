namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class MarginalNode : INodeRule
{
    public string Name => "marginal";
    public string XPath => "//marginal";
    public string[]? Attributes { get; } = { "index", "letter", "page", "line" };
    public string? uniquenessAttribute => "index";
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}