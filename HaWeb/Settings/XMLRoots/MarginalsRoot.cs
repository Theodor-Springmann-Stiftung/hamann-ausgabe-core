namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.XMLParser;

public class MarginalsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Stellenkommentar";
    public string Prefix { get; } = "stellenkommentar";
    public string[] XPathContainer { get; } = { ".//data/marginalien", ".//marginalien" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "marginal") return true;
        else return false;
    };

    public Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("index");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        else return null;
    };

    public List<(string, string?)>? GenerateFields(XMLRootDocument document) {
        return null;
    }

    public (string?, string?) GenerateIdentificationString(XElement element) {
        return (null, null);
    }

    public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
        return true;
    }

    public XElement CreateHamannDocument(XElement element) {
        var opus = new XElement("opus");
        opus.AddFirst(element);
        return opus;
    }

}