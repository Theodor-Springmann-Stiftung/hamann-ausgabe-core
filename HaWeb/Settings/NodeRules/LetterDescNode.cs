namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterDescNode : INodeRule
{
    public string Name => "letterDesc";
    public string XPath => "//letterDesc";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => "ref" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}