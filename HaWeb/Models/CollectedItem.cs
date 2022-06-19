namespace HaWeb.Models;
using HaWeb.SearchHelpers;
using HaWeb.XMLParser;
using System.Xml.Linq;

public class CollectedItem : ISearchable {
    public string Index { get; private set; }
    public string Collection { get; private set; }
    public string? SearchText { get; private set; }
    public XElement ELement { get; private set; }
    public IXMLRoot Root { get; private set; }

    public CollectedItem(string index, XElement element, IXMLRoot root, string collection, string? searchtext = null) {
        this.Index = index;
        this.SearchText = searchtext;
        this.Collection = collection;
        this.Root = root;
        this.ELement = element;
    }
}