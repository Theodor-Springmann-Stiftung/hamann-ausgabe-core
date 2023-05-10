namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterTextNode : INodeRule
{
    public string Name => "letterText";
    public string XPath => "//letterText";
    public string[]? Attributes { get; } = { "index" };
    public string? uniquenessAttribute => "index" ;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
    };
}