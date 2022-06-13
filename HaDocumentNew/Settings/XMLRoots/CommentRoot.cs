namespace HaDocument.Settings.XMLRoots;
using System.Xml.Linq;

public class CommentRoot : HaDocument.Interfaces.IXMLRoot {
    public string Type { get; } = "Register";
    public string Prefix { get; } = "register";
    public string[] XPathContainer { get; } = { ".//data//kommentare/kommcat", ".//kommentare/kommcat" };

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

    // public List<(string, string?)>? GenerateFields(XMLRootDocument document) {
    //     return null;
    // }

    public (string?, string?) GenerateIdentificationString(XElement element) {
        var kat = element.Attribute("value");
        if (kat != null && !String.IsNullOrWhiteSpace(kat.Value))
            return (null, kat.Value);
        return (null, null);
    }

    // public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
    //     return true;
    // }

    public XElement CreateHamannDocument(XElement element) {
        var opus = new XElement("opus");
        var kommentare = new XElement("kommentare");
        kommentare.AddFirst(element);
        opus.AddFirst(kommentare);
        return opus;
    }

    // public void MergeIntoFile(XElement file, XMLRootDocument document) {
    //     if (file.Element("kommentare") == null)
    //         file.AddFirst(new XElement("kommentare"));
    //     file.Element("kommentare")!.AddFirst(document.Root);
    // }

}