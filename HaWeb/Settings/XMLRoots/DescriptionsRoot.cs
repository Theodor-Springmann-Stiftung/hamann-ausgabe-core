namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.XMLParser;

public class DescriptionsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Metadaten";
    public string Container { get; } = "descriptions";

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "letterDesc") return true;
        return false;
    };

    public Func<XElement, string?> GetKey { get; } = (elem) => { 
        var index = elem.Attribute("ref");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        else return null;
    };

    public List<(string, string)>? GenerateFields(XMLRootDocument document) {
        return null;
    }

    public (string?, string) GenerateIdentificationString(XElement element) {
        return (null, Container);
    }

    public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
        return true;
    }

}