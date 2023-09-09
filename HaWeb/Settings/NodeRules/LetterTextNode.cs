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
    public string[]? Attributes { get; } = { "index" };
    public string? uniquenessAttribute => "index" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}