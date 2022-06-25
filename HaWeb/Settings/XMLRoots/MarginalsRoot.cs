namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class MarginalsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Stellenkommentar";
    public string Prefix { get; } = "stellenkommentar";
    public string[] XPathContainer { get; } = { ".//data/marginalien", ".//marginalien" };
    public (
        string Key, 
        string xPath,
        Func<XElement, string?> GenerateKey,
        Func<XElement, Dictionary<string, string[]>?>? GenerateDataFields,
        Func<List<CollectedItem>, Dictionary<string, Lookup<string, CollectedItem>>?>? GroupingsGeneration,
        Func<List<CollectedItem>, Dictionary<string, List<CollectedItem>>?>? SortingsGeneration,
        bool Searchable
    )[]? Collections { get; } = { 
        ("marginals", "/data/marginalien/marginal", GetKey, null, null, null, true),
        ("marginals", "/marginalien/marginal", GetKey, null, null, null, true)
    };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "marginal") return true;
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
        if (file.Element("marginalien") == null)
            file.AddFirst(new XElement("marginalien"));
        var elements = document.GetElement().Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("marginalien");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }

}