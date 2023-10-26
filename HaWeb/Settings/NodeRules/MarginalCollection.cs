using HaWeb.Models;
using System.Xml.Linq;
using HaWeb.XMLTests;
namespace HaWeb.Settings.NodeRules;

public class MarginalCollectionRules : ICollectionRule {
    public string Name { get; } = "marginal";
    public HamannXPath[] Bases { get; } = { 
        new HamannXPath() { Documents = new[] { "stellenkommentar" }, XPath = "//marginal"}
    };
    public HamannXPath[] Backlinks { get; } = {};

    public IEnumerable<(string, XElement, XMLRootDocument)> GenerateIdentificationStrings(IEnumerable<(XElement, XMLRootDocument)> list) {
        foreach (var e in list) {
            var id = e.Item1.Attribute("letter")!.Value;
            id += "-";
            id += e.Item1.Attribute("page")!.Value;
            id += "-";
            id += e.Item1.Attribute("line")!.Value;
            if (e.Item1.HasAttributes && e.Item1.Attribute("sort") != null) {
                id += "-";
                id += e.Item1.Attribute("sort")!.Value;
            }
            yield return (
                id,
                e.Item1,
                e.Item2
            );
        }
    }

    public IEnumerable<(string, XElement, XMLRootDocument, bool)> GenerateBacklinkString(IEnumerable<(XElement, XMLRootDocument)> list) => null;


    public bool CheckDatatypes(XElement element) {
        if (element.HasAttributes && element.Attribute("sort") != null ) {
            return Int32.TryParse(element.Attribute("sort").Value, out var _);
        } else {
            return true;
        }
    }
}
