namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class MarginalsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Stellenkommentar";
    public string Prefix { get; } = "stellenkommentar";
    public string[] XPathContainer { get; } = { ".//data/marginalien", ".//marginalien" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "marginal") return true;
        else return false;
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

    public void MergeIntoFile(XElement file, XMLRootDocument document) {
        if (file.Element("marginalien") == null)
            file.AddFirst(new XElement("marginalien"));
        var elements = document.GetElement().Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("marginalien");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}