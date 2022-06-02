namespace HaWeb.XMLParser;
using System.Xml.Linq;

public class XMLRootDocument {
    public string Type {get; private set; }
    public DateTime Date { get; private set; }
    public XElement Root { get; private set; }
    public (string?, string) IdentificationString { get; private set; }

    public Dictionary<string, XElement>? Elements { get; set; }
    public List<(string, string)>? Fields { get; set; }

    public XMLRootDocument(string type, (string?, string) idString, XElement root) {
        Type = type;
        IdentificationString = idString;
        Date = DateTime.Today;
        Root = root;
    }

    public string CreateFilename() {
        var filename = _removeInvalidChars(Type) + "_";
        if (IdentificationString.Item1 != null) filename += _removeInvalidChars(IdentificationString.Item1) + "_"; 
        filename += _removeInvalidChars(IdentificationString.Item2) + "_";
        filename += _removeInvalidChars(Date.Year.ToString() + "-" + Date.Month.ToString() + "-" + Date.Day.ToString());
        return filename;
    }

    private string _removeInvalidChars(string? s) {
        if (String.IsNullOrWhiteSpace(s)) return "";
        foreach (var c in Path.GetInvalidFileNameChars()) {
            s = s.Replace(c, '-');
        }
        return s;
    }

    public async void Save(Stream stream) {
        var nr = new XElement("opus");
        nr.AddFirst(Root);
        await nr.SaveAsync(stream, SaveOptions.DisableFormatting, new CancellationToken());
    }
}