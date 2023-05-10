namespace HaWeb.Models;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using HaWeb.XMLParser;
using System.Text;

public class XMLRootDocument {
    private XElement? _Element;
    private string? _filename;
    private IFileInfo? _file;

    private StringBuilder? _log;

    [JsonIgnore]
    public IXMLRoot XMLRoot { get; private set; }

    public string FileName {
        get {
            if (_filename == null)
                _filename = _CreateFilename();
            return _filename;
        }
    }

    [JsonIgnore]
    public IFileInfo? File { 
        get {
            return _file;
        } 
        set {
            _file = value;
            // After saving, we don't need to save the ELement anymore, it can get read in if it's used.
            // We do this to prevent memory hogging. TODO: MAKE IT MORE EFFICIENT, EG ALL USED FILES HAVE SET ELEMENTS OR SO
            // if (value != null) _Element = null;
        } }
    public string Prefix { get; private set; }
    public DateTime Date { get; private set; }

    public (string?, string?) IdentificationString { get; private set; }
    public List<(string, string?)>? Fields { get; set; }

    // Entry point for file reading
    public XMLRootDocument(IXMLRoot xmlRoot, IFileInfo file) {
        XMLRoot = xmlRoot;
        Prefix = xmlRoot.Prefix;
        File = file;
        Date = file.LastModified.LocalDateTime;
        _filename = file.Name;
        _GenerateFieldsFromFilename(file.Name);
    }

    // Entry point for XML upload reading
    public XMLRootDocument(IXMLRoot xmlRoot, string prefix, (string?, string?) idString, XElement element) {
        XMLRoot = xmlRoot;
        Prefix = prefix;
        IdentificationString = idString;
        Date = DateTime.Now;
        _Element = element;
    }

    private string _CreateFilename() {
        var filename = _removeInvalidChars(Prefix) + "_";
        if (!String.IsNullOrWhiteSpace(IdentificationString.Item1)) {
            var hash = IdentificationString.Item1.GetHashCode().ToString("X8");
            filename += hash + "_";
        }
        if (!String.IsNullOrWhiteSpace(IdentificationString.Item2)) filename += _removeInvalidChars(IdentificationString.Item2) + "_";
        filename += _removeInvalidChars(Date.Year.ToString() + "-" + Date.Month.ToString() + "-" + Date.Day.ToString()) + "." +  Path.GetRandomFileName();
        return filename + ".xml";
    }

    private string _removeInvalidChars(string? s) {
        if (String.IsNullOrWhiteSpace(s)) return string.Empty;
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

    public string? GetLog() {
        if (_log == null) return null;
        return _log.ToString();
    }

    public void Log(string msg) {
        if (_log == null) _log = new StringBuilder();
        var prefix = DateTime.Now.ToString() + " ";
        if (File != null) prefix += File.Name + ": ";
        _log.Append("<br>" + prefix + msg);
    }

    public void ResetLog() {
        if (_log != null) _log.Clear();
    }

    // Call on UnUse to prevent memory hogging
    public void UnUse() {
        _Element = null;
        _log = null;
    }

    public XElement GetElement() {
        if (_Element == null)
            _Element = _GetElement();
        return _Element;
    }

    private XElement _GetElement() {
        if (File == null || String.IsNullOrWhiteSpace(File.PhysicalPath) || !File.Exists)
            throw new Exception("Es ist kein Pfad für die XML-Datei vorhanden.");

        if (XMLRoot == null)
            throw new Exception("Kein gültiges Hamann-Dokument: " + File.PhysicalPath + "Vom Prefix: " + Prefix);

        XDocument? doc = null;
        try {
            doc = XDocument.Load(File.PhysicalPath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        } catch (Exception ex) {
            throw new Exception("Fehler beim Lesen des Dokuments: " + ex.Message);
        }

        if (doc == null || doc.Root == null)
            throw new Exception("Das Dokument ist ungültig und kann nicht gelesen werden: " + File.PhysicalPath);

        var element = XMLRoot.IsTypeOf(doc.Root);
        if (element == null || !element.Any())
            throw new Exception("Kein gültiges Hamann-Dokument: " + File.PhysicalPath + "Vom Prefix: " + Prefix);

        return element.First();
    }

    public async Task Save(Stream stream, ModelStateDictionary state) {
        if (XMLRoot == null) {
            state.AddModelError("Error", "No corresponding Root Element found.");
            return;
        }

        if (_Element == null) {
            if (File == null)  {
                state.AddModelError("Error", "There is neither a file nor a saved element for this Document aborting the save.");
                return;
            }
            _Element = GetElement();
        }
        
        await XMLRoot.CreateHamannDocument(_Element).SaveAsync(stream, SaveOptions.DisableFormatting, new CancellationToken());
    }
}