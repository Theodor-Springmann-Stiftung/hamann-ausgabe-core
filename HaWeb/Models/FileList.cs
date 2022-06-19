namespace HaWeb.Models;
using HaWeb.XMLParser;
using System.Text.Json.Serialization;

public class FileList {
    private HashSet<XMLRootDocument>? _Files;

    [JsonIgnore]
    public IXMLRoot XMLRoot { get; private set; }

    public FileList(IXMLRoot xmlRoot) {
        XMLRoot = xmlRoot;
    }

    public void Add(XMLRootDocument document) {
        if (document.Prefix != XMLRoot.Prefix)
            throw new Exception("Diese Liste kann nur Elemente des Typs " + XMLRoot.Prefix + " enthalten");

        if (_Files == null) _Files = new HashSet<XMLRootDocument>();
        var replacing = _Files.Where(x => x.FileName == document.FileName);
        if (replacing != null && replacing.Any()) _Files.Remove(replacing.First());
        _Files.Add(document);
    }

    public bool Contains(XMLRootDocument doc) {
        if (_Files == null) return false;
        return _Files.Contains(doc);
    }

    public List<XMLRootDocument>? GetFileList()
        => this._Files != null ? this._Files.ToList() : null;

    public FileList Clone() {
        var ret = new FileList(this.XMLRoot);
        if (_Files != null)
            foreach (var file in _Files) {
                ret.Add(file);
            }
        return ret;
    }
}