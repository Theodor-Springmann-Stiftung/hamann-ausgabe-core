namespace HaWeb.XMLParser;
using System.Xml.Linq;
using System.Xml.XPath;

public interface IXMLRoot {
    // Name of the IXMLRoot
    public abstract string Type { get; }

    // Name of the file prefix
    public abstract string Prefix { get; }
    
    // XPaths to determine if container is present
    public abstract string[] XPathContainer { get; }

    // Tag Name of child objects to be collected 
    public abstract Predicate<XElement> IsCollectedObject { get; }

    // Gets the Key of a collected object
    public abstract Func<XElement, string?> GetKey { get; }

    // Can the Root be found within that document? 
    public List<XElement>? IsTypeOf(XElement root) {
        List<XElement>? ret = null;
        foreach (var p in XPathContainer) {
            var elements = root.XPathSelectElements(p);
            if (elements != null && elements.Any()) {
                if (ret == null) ret = new List<XElement>();
                ret.AddRange(elements);
            }
        }
        return ret;
    }

    // Generate certain metadat fields to display about this root
    public abstract List<(string, string?)>? GenerateFields(XMLRootDocument document);

    // Generate an identification string of which the hash will be the filename. 
    // The second string will be appended literally for convenience.
    // If the queries of two document are equal they replace each other
    // If the queries and the date of two documents are equal the later one gets deleted
    public abstract (string?, string?) GenerateIdentificationString(XElement element);

    // Further deciding which of two documents replaces which
    public abstract bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2);

    public Dictionary<string, XElement>? GetCollectedObjects(XMLRootDocument document) {
        Dictionary<string, XElement>? ret = null;
        var root = document.Root;
        root.Elements().Where(x => this.IsCollectedObject(x)).ToList().ForEach(x => {
            var id = this.GetKey(x);
            if (id != null) {
                if (ret == null) ret = new Dictionary<string, XElement>();
                ret.Add(id, x);
            }
        });
        return ret;
    }

    public abstract XElement CreateHamannDocument(XElement element);
}