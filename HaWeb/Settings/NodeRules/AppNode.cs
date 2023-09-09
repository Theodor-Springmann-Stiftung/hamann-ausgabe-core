namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class AppNode : INodeRule
{
    public string Name => "app";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "ueberlieferung" },
        XPath = "//app"
    };
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
        ("ref", new HamannXPath() { Documents = new[] { "personenorte" }, XPath =  "//appDef" }, "index")
    };
}