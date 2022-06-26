using System.Reflection.Emit;
using System.Collections;
namespace HaWeb.Models;
using HaWeb.SearchHelpers;
using HaWeb.XMLParser;
using System.Xml.Linq;

public class CollectedItem : ISearchable {
    public string Index { get; private set; }
    public string? SearchText { get; private set; }
    public IDictionary<string, string>? Fields { get; private set; }
    public XElement ELement { get; private set; }
    public IXMLCollection Collection { get; private set; }

    public CollectedItem(
        string index, 
        XElement element, 
        IXMLCollection collection, 
        IDictionary<string, string>? fields, 
        string? searchtext = null
    ) {
        this.Index = index;
        this.SearchText = searchtext;
        this.Collection = collection;
        this.ELement = element;
        this.Fields = fields;
    }

    public string? this[string v] {
        get {
            if (Fields != null && Fields.ContainsKey(v))
                return Fields[v];
            return null;
        }
    }
}