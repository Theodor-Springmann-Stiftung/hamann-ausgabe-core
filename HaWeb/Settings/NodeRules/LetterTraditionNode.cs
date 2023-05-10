namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterTraditionNode : INodeRule
{
    public string Name => "letterTradition";
    public string XPath => "//letterTradition";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => "ref" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}