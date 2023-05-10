namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class KommentarNode : INodeRule
{
    public string Name => "kommentar";
    public string XPath => "//kommentar";
    public string[]? Attributes { get; } = { "id" };
    public string? uniquenessAttribute => "id" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}