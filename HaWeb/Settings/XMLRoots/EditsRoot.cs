namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class EditsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Texteingriffe";
    public string Prefix { get; } = "texteingriffe";
    public string[] XPathContainer { get; } = { "/opus/data/edits", "/opus/edits" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "editreason") return true;
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
        if (file.Element("edits") == null)
            file.AddFirst(new XElement("edits"));
        var elements = document.Element.Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("edits");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}