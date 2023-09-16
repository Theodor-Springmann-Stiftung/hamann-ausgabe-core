using HaWeb.Models;
using System.Xml.Linq;
using HaWeb.XMLTests;
namespace HaWeb.Settings.NodeRules;

public class StructureCollection : ICollectionRule {
    public string Name { get; } = "structure";
    public HamannXPath[] Bases { get; } = { 
        new HamannXPath() { Documents = new[] { "brieftext" }, XPath = "//letterText" },
        new HamannXPath() { Documents = new[] { "ueberlieferung" }, XPath = "//letterTradition"}
    };
    public HamannXPath[] Backlinks { get; } = {
        new HamannXPath() { Documents = new[] { "stellenkommentar", "ueberlieferung", "texteingriffe", "register" }, XPath =  "//intlink" },
        new HamannXPath() { Documents = new[] { "stellenkommentar" }, XPath = "//marginal"}
    };

    public IEnumerable<(string, XElement, XMLRootDocument)> GenerateIdentificationStrings(IEnumerable<(XElement, XMLRootDocument)> list) {
        foreach (var e in list) {
            var id = e.Item1.Attribute("letter")!.Value;
            var currpage = String.Empty;
            var currline = String.Empty;
            foreach (var el in e.Item1.Descendants()) {
                if (el.Name == "page" && el.Attribute("index") != null) currpage = el.Attribute("index")!.Value;
                if (el.Name == "line" && el.Attribute("index") != null) {
                    currline = el.Attribute("index")!.Value;
                    yield return (
                        id + "-" + currpage + "-" + currline, 
                        e.Item1, 
                        e.Item2);
                }
            }
        }
    }
    public IEnumerable<(string, XElement, XMLRootDocument, bool)> GenerateBacklinkString(IEnumerable<(XElement, XMLRootDocument)> list) {
        foreach (var e in list) {
            var letter = e.Item1.Attribute("letter") != null ? e.Item1.Attribute("letter")!.Value : "NA";
            var page = e.Item1.Attribute("page") != null ? e.Item1.Attribute("page")!.Value : "NA";
            var line = e.Item1.Attribute("line") != null ? e.Item1.Attribute("line")!.Value : "NA";
            var partialmatch = e.Item1.Name == "marginal" ? false : true;
            yield return (
                letter + "-" + page + "-" + line,
                e.Item1,
                e.Item2,
                partialmatch);
        }
    }
}
