namespace HaWeb.Settings.NodeRules;

using System.Collections.Generic;
using HaWeb.XMLTests;

public class EditNode : INodeRule
{
    public string Name => "edit";
    public string XPath => "//edit";
    public string[]? Attributes { get; } = { "ref" };
    public string? uniquenessAttribute => null;
    public List<(string, string, string)>? References { get; } = new List<(string, string, string)>()
    {
        ("ref", "//editreason", "index")
    };
}