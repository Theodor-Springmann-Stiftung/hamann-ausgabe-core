namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class EditNode : INodeRule
{
    public string Name => "edit";
    public HamannXPath XPath => new HamannXPath() {
        Documents = new[] { "brieftext", "texteingriffe", "ueberlieferung" },
        XPath = "//edit"
    };
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, HamannXPath, string)>? References { get; } = new List<(string, HamannXPath, string)>()
    {
        ("ref", new HamannXPath() { Documents = new[] { "texteingriffe" }, XPath =  "//editreason" }, "index")
    };
}