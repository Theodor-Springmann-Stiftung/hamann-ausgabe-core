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
    public string[]? Attributes { get; } = { "letter" };
    public string? uniquenessAttribute => "letter" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}