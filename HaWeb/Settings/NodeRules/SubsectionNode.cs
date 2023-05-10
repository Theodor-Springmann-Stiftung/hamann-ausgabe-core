namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class SubsectionNode : INodeRule
{
    public string Name => "subsection";
    public string XPath => "//subsection";
    public string[]? Attributes { get; } = { "id" };
    public string? uniquenessAttribute => "id" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}