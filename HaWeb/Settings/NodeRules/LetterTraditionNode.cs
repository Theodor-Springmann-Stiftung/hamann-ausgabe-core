namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterTraditionNode : INodeRule
{
    public string Name => "letterTradition";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "ueberlieferung" },
        XPath = "//letterTradition"
    };
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => "ref" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}