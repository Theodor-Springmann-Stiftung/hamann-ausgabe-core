namespace HaWeb.XMLParser;
using HaWeb.Models;
using System.Xml.Linq;

public interface IXMLCollection {
    // Collections of Elements to be created a Hamann File Root
    // Key: the key under which the element(s) will be filed
    // xPath: (absolute, realtive if subelement) XPaths to the element(s)
    // GenerateKey: How to extract an identifier for the single element in the collection
    // GenerateDataFields: Generate a dict of data associated with each of the collected Elements input: XElement output: Dictonary<string>
    // GroupingsGeneration: datafields by which dictorary-like groups should be held in memory input: List<CollectedItem> output: Dictonary<string, Lookup<string, CollectedItem[]>>
    // SortingsGeneration: datafields by which a sorting should be held in memory input: List<CollectedItem> output: ordered List<CollectedItem>
    // SubCollections to be created in this element
    // Searchable: Will the element be indexed for full-text-search?

    abstract string Key { get; }
    abstract string[] xPath { get; }
    abstract Func<XElement, string?> GenerateKey { get; }
    abstract Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; }
    abstract Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; }
    abstract Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; }
    abstract IXMLCollection[]? SubCollections { get; }
    abstract bool Searchable { get; }
}