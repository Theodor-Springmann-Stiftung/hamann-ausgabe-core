namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;
using System.IO;

public class DocumentRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Brieftext";
    public string Prefix { get; } = "brieftext";
    public string[] XPathContainer { get; } = { ".//data/document", ".//document" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "letterText") return true;
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

    public void MergeIntoFile(XElement file, XMLRootDocument document) {
        if (file.Element("document") == null)
            file.AddFirst(new XElement("document"));
        var elements = document.Root.Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("document");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}