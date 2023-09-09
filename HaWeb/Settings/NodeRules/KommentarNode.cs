namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class KommentarNode : INodeRule
{
    public string Name => "kommentar";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "register" },
        XPath = "//kommentar"
    };
    public string[]? Attributes { get; } = { "id" };
    public string? uniquenessAttribute => "id" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}