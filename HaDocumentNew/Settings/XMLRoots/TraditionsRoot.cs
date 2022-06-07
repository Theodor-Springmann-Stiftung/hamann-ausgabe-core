namespace HaDocument.Settings.XMLRoots;
using System.Xml.Linq;


public class TraditionsRoot : HaWeb.XMLParser.IXMLRoot {
    public string Type { get; } = "Überlieferung";
    public string Prefix { get; } = "ueberlieferung";
    public string[] XPathContainer { get; } = { ".//data/traditions", ".//traditions" };

    public Predicate<XElement> IsCollectedObject { get; } = (elem) => {
        if (elem.Name == "letterTradition") return true;
        else return false;
    };

    public Func<XElement, string?> GetKey { get; } = (elem) => {
        var index = elem.Attribute("ref");
        if (index != null && !String.IsNullOrWhiteSpace(index.Value))
            return index.Value;
        else return null;
    };

    public List<(string, string?)>? GenerateFields(XMLRootDocument document) {
        return null;
    }

    public (string?, string?) GenerateIdentificationString(XElement element) {
        return (null, null);
    }

    public bool Replaces(XMLRootDocument doc1, XMLRootDocument doc2) {
        return true;
    }

    public XElement CreateHamannDocument(XElement element) {
        var opus = new XElement("opus");
        opus.AddFirst(element);
        return opus;
    }

    public void MergeIntoFile(XElement file, XMLRootDocument document) {
        if (file.Element("traditions") == null)
            file.AddFirst(new XElement("traditions"));
        var elements = document.Root.Elements().Where(x => IsCollectedObject(x));
        var root = file.Element("traditions");
        foreach (var element in elements) {
            root!.Add(element);
        }
    }
}