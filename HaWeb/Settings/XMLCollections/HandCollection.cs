using System.Xml.Linq;
using HaWeb.Models;

public class HandCollection : HaWeb.XMLParser.IXMLCollection {
    public string Key { get; } = "hands";
    public string[] xPath { get; } = new string[] { 
        "/opus/data/document/letterText//hand", 
        "/opus/document/letterText//hand", 
        "/opus/data/traditions/letterTradition//hand", 
        "/opus/traditions/letterTradition//hand"
    };
    public Func<XElement, string?> GenerateKey { get; } = GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = null;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        // TODO IMPLEMENT
        return null;
    };
}