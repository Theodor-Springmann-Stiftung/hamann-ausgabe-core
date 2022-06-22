namespace HaWeb.XMLParser;
using System.Xml.Linq;
using System.Xml.XPath;
using HaWeb.Models;

public interface IXMLRoot {
    // Name of the IXMLRoot
    public abstract string Type { get; }

    // Name of the file prefix
    public abstract string Prefix { get; }

    // XPaths to determine if container is present
    public abstract string[] XPathContainer { get; }

    // Collections of Elements to be created from this Root
    // Key: the key under which the element(s) will be files
    // xPath: the (absolute) XPath to the element(s)
    // KeyFunc: How to extrect an identifier for the single element in the collection
    // LookupsFunc: Function to generate metadata fields for the object, which will then in turn be a possibility to seach and filter without parsing
    // Searchable: Will the element be indexed for full-text-search?
    public abstract (
        string Key, 
        string xPath,
        Func<XElement, string?> KeyFunc, 
        // Func<XElement, Dictionary<string, string[]>> LookupsFunc,
        bool Searchable
    )[]? XPathCollection { get; }

    // Determines child objects to be collected 
    // (deprecated see collections above; only used internally)
    public abstract Predicate<XElement> IsCollectedObject { get; }

    // Gets the Key of a collected object
    // public abstract Func<XElement, string?> GetKey { get; }

    // Can the Root be found within that document? 
    public List<XElement>? IsTypeOf(XElement root) {
        List<XElement>? ret = null;
        foreach (var p in XPathContainer) {
            var elements = root.XPathSelectElements(p);
            if (elements != null && elements.Any()) {
                if (ret == null) ret = new List<XElement>();
                foreach (var e in elements)
                    if (!ret.Contains(e)) ret.Add(e);
            }
        }
        return ret;
    }

    // Generate certain metadata fields to display about this root
    public abstract List<(string, string?)>? GenerateFields(XMLRootDocument document);

    // Generate an identification string of which the hash will be the filename. 
    // The second string will be appended literally for convenience.
    // If the queries of two document are equal they replace each other
    // If the queries and the date of two documents are equal the later one gets deleted
    public abstract (string?, string?) GenerateIdentificationString(XElement element);

    // Further deciding which of two documents replaces which
    public abstract bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2);

    // public Dictionary<string, XElement>? GetCollectedObjects(XMLRootDocument document) {
    //     Dictionary<string, XElement>? ret = null;
    //     var root = document.GetElement();
    //     root.Elements().Where(x => this.IsCollectedObject(x)).ToList().ForEach(x => {
    //         var id = this.GetKey(x);
    //         if (id != null) {
    //             if (ret == null) ret = new Dictionary<string, XElement>();
    //             ret.Add(id, x);
    //         }
    //     });
    //     return ret;
    // }

    public abstract XElement CreateHamannDocument(XElement element);

    public abstract void MergeIntoFile(XElement file, XMLRootDocument document);
}