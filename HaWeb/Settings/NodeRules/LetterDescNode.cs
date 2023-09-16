namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class LetterDescNode : INodeRule
{
    public string Name => "letterDesc";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "metadaten" },
        XPath = "//letterDesc"
    };
    public string[]? Attributes { get; } = { "letter" };
    public string? uniquenessAttribute => "letter" ;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
    };
}