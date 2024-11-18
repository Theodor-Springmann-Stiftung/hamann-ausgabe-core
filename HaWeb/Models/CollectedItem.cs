namespace HaWeb.Models;
using HaWeb.SearchHelpers;
using HaWeb.XMLParser;
using System.Xml.Linq;

public class CollectedItem : ISearchable {
    public string ID { get; private set; }
    public string? SearchText { get; private set; }
    public IDictionary<string, string>? Fields { get; private set; }
    public XElement Element { get; private set; }
    public IXMLCollection Collection { get; private set; }
    public IDictionary<string, CollectedItem>? Items { get; set; }

    public CollectedItem(
        string id,
        XElement element,
        IXMLCollection collection,
        string? searchtext = null
    ) {
        this.ID = id;
        this.SearchText = searchtext;
        this.Collection = collection;
        this.Element = element;
    }

    public string? this[string v] {
        get {
            if (Fields == null && Collection.GenerateDataFields != null)
                Fields = Collection.GenerateDataFields(this.Element);
            if (Fields != null && Fields.ContainsKey(v))
                return Fields[v];
            return null;
        }
    }
}
