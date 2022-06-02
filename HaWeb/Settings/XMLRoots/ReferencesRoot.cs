namespace HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using HaWeb.XMLParser;

public class ReferencesRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Personen / Orte";
    public string Container { get; } = "definitions";

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "personDefs" || elem.Name == "structureDefs" || elem.Name == "handDefs" || elem.Name == "locationDefs")
            return true;
        return false;
    };

    public Func<XElement, string?> GetKey { get; } = (elem) => { 
        return elem.Name.ToString();
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