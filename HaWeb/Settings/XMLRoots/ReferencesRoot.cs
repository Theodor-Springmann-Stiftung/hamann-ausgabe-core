namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class ReferencesRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Personen / Orte";
    public string Prefix { get; } = "personenorte";
    public string[] XPathContainer { get; } = { "/opus/data/definitions", "/opus/definitions" };
    
    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "personDefs" || elem.Name == "structureDefs" || elem.Name == "handDefs" || elem.Name == "locationDefs" || elem.Name == "appDefs")
            return true;
        return false;
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
        if (file.Element("definitions") == null)
            file.AddFirst(new XElement("definitions"));
        var elements = document.Element.Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("definitions");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}