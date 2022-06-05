namespace HaWeb.Models;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using HaWeb.XMLParser;

public class XMLRootDocument {
    private XElement? _Element;
    private string? _filename;
    public IXMLRoot XMLRoot { get; private set; }

    [JsonIgnore]
    public XElement Root {
        get {
            if (_Element == null) {
                _Element = _GetElement();
            }
            return _Element;
        }
    }

    public string FileName {
        get {
            if (_filename == null)
                _filename = _CreateFilename();
            return _filename;
        }
    }

    [JsonIgnore]
    public IFileInfo? File { get; private set; }
    public string Prefix { get; private set; }
    public DateTime Date { get; private set; }

    public (string?, string?) IdentificationString { get; private set; }
    public List<(string, string?)>? Fields { get; set; }

    // Entry point for file reading
    public XMLRootDocument(IXMLRoot xmlRoot, IFileInfo file) {
        XMLRoot = xmlRoot;
        Prefix = xmlRoot.Prefix;
        SetFile(file);
    }

    // Entry point for XML upload reading
    public XMLRootDocument(IXMLRoot xmlRoot, string prefix, (string?, string?) idString, XElement element) {
        XMLRoot = xmlRoot;
        Prefix = prefix;
        IdentificationString = idString;
        Date = DateTime.Today;
        _Element = element;
    }

    public void SetFile(IFileInfo file) {
        File = file;
        Date = file.LastModified.DateTime;
        _GenerateFieldsFromFilename(file.Name);
    }

    private string _CreateFilename() {
        var filename = _removeInvalidChars(Prefix) + "_";
        if (!String.IsNullOrWhiteSpace(IdentificationString.Item1)) {
            var hash = IdentificationString.Item1.GetHashCode().ToString("X8");
            filename += hash + "_";
        }
        if (!String.IsNullOrWhiteSpace(IdentificationString.Item2)) filename += _removeInvalidChars(IdentificationString.Item2) + "_";
        filename += _removeInvalidChars(Date.Year.ToString() + "-" + Date.Month.ToString() + "-" + Date.Day.ToString());
        return filename + ".xml";
    }

    private string _removeInvalidChars(string? s) {
        if (String.IsNullOrWhiteSpace(s)) return "";
        foreach (var c in Path.GetInvalidFileNameChars()) {
            s = s.Replace(c, '-');
        }
        s = s.Replace('_', '-');
        return s;
    }

    private void _GenerateFieldsFromFilename(string filename) {
        var split = filename.Split('_');
        Prefix = split[0];
        if (split.Length == 3) {
            IdentificationString = (null, split[1]);
        } else if (split.Length == 4) {
            IdentificationString = (split[1], split[2]);
        } else {
            IdentificationString = (null, null);
        }
    }

    private XElement _GetElement() {
        if (File == null || String.IsNullOrWhiteSpace(File.PhysicalPath) || !File.Exists)
            throw new Exception("Es ist kein Pfad für die XML-Datei vorhanden.");

        var root = XMLRoot;
        if (root == null)
            throw new Exception("Kein gültiges Hamann-Dokument: " + File.PhysicalPath + "Vom Prefix: " + Prefix);

        XDocument? doc = null;
        try {
            doc = XDocument.Load(File.PhysicalPath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        } catch (Exception ex) {
            throw new Exception("Fehler beim Lesen des Dokuments: " + ex.Message);
        }

        if (doc == null || doc.Root == null)
            throw new Exception("Das Dokument ist ungültig und kann nicht gelesen werden: " + File.PhysicalPath);

        var element = root.IsTypeOf(doc.Root);
        if (element == null || !element.Any())
            throw new Exception("Kein gültiges Hamann-Dokument: " + File.PhysicalPath + "Vom Prefix: " + Prefix);

        return element.First();
    }

    public async Task Save(Stream stream, ModelStateDictionary state) {
        var root = XMLRoot;
        if (root == null) {
            state.AddModelError("Error", "No corresponding Root Element found.");
            return;
        }
        await root.CreateHamannDocument(Root).SaveAsync(stream, SaveOptions.DisableFormatting, new CancellationToken());
    }
}