namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class DescriptionsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Metadaten";
    public string Prefix { get; } = "metadaten";
    public string[] XPathContainer { get; } = { ".//data/descriptions", ".//descriptions" };
    public (string Key, string xPath, Func<XElement, string?> KeyFunc, bool Searchable)[]? XPathCollection { get; } = {
        ("metas", "/opus/descriptions/letterDesc", GetKey, false),
        ("metas", "/opus/data/descriptions/letterDesc", GetKey, false)
    };
    
    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "letterDesc") return true;
        return false;
    };

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("ref");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        return null;
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
        if (file.Element("descriptions") == null)
            file.AddFirst(new XElement("descriptions"));
        var elements = document.GetElement().Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("descriptions");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}