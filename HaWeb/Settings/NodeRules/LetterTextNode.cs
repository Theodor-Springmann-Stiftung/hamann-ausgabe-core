namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterTextNode : INodeRule
{
    public string Name => "letterText";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "brieftext" },
        XPath = "//letterText"
    };
    public string[]? Attributes { get; } = { "letter" };
    public string? uniquenessAttribute => "letter" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}