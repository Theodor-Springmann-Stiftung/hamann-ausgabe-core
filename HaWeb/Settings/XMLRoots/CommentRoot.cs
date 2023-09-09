namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.Models;
using HaWeb.XMLParser;

public class CommentRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Register";
    public string Prefix { get; } = "register";
    public string[] XPathContainer { get; } = { "/opus/data//kommentare/kommcat", "/opus//kommentare/kommcat" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "kommentar") return true;
        else return false;
    };

    public List<(string, string?)>? GenerateFields(XMLRootDocument document) {
        return null;
    }

    public (string?, string?) GenerateIdentificationString(XElement element) {
        var kat = element.Attribute("value");
        if (kat != null && !String.IsNullOrWhiteSpace(kat.Value))
            return (null, kat.Value);
        return (null, null);
    }

    public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
        return true;
    }

    public XElement CreateHamannDocument(XElement element) {
        var opus = new XElement("opus");
        var kommentare = new XElement("kommentare");
        kommentare.AddFirst(element);
        opus.AddFirst(kommentare);
        return opus;
    }

    public void MergeIntoFile(XElement file, XMLRootDocument document) {
        if (file.Element("kommentare") == null)
            file.AddFirst(new XElement("kommentare"));
        file.Element("kommentare")!.AddFirst(document.Element);
    }

}