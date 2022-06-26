namespace HaWeb.Settings.XMLCollections;
using HaWeb.Models;
using System.Xml.Linq;

public class MetaCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "metas";
    public string[] xPath { get; } = new string[] { "/opus/descriptions/letterDesc",  "/opus/data/descriptions/letterDesc" };
    public Func<XElement, string?> GenerateKey { get; } = GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = false;

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("ref");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        return null;
    };
}