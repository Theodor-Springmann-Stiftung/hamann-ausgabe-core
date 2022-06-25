namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class EditsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Texteingriffe";
    public string Prefix { get; } = "texteingriffe";
    public string[] XPathContainer { get; } = { ".//data/edits", ".//edits" };
    public (
        string Key, 
        string xPath,
        Func<XElement, string?> GenerateKey,
        Func<XElement, Dictionary<string, string[]>?>? GenerateDataFields,
        Func<List<CollectedItem>, Dictionary<string, Lookup<string, CollectedItem>>?>? GroupingsGeneration,
        Func<List<CollectedItem>, Dictionary<string, List<CollectedItem>>?>? SortingsGeneration,
        bool Searchable
    )[]? Collections { get; } = { 
        ("edits", "/data/edits/editreason", GetKey, null, null, null, true),
        ("edits", "/edits/editreason", GetKey, null, null, null, true)
    };
    
    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "editreason") return true;
        else return false;
    };

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
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
        if (file.Element("edits") == null)
            file.AddFirst(new XElement("edits"));
        var elements = document.GetElement().Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("edits");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}