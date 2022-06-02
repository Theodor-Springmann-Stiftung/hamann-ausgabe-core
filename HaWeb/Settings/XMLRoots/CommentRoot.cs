namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.XMLParser;

public class CommentRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Register";
    public string Container { get; } = "kommcat";

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "kommentar") return true;
        else return false;
    };

    public Func<XElement, string?> GetKey { get; } = (elem) => { 
        var index = elem.Attribute("id");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        else return null;
    };

    public List<(string, string)>? GenerateFields(XMLRootDocument document) {
        return null;
    }

    public (string?, string) GenerateIdentificationString(XElement element) {
        var kat = element.Attribute("value");
        if (kat != null && !String.IsNullOrWhiteSpace(kat.Value)) 
            return (null, kat.Value);
        return (null, Container);
    }

    public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
        return true;
    }

}