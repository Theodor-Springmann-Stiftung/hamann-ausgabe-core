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
        if (!_Files.Contains(document)) _Files.Add(document);
    }

    public List<XMLRootDocument>? GetFileList()
        => this._Files != null ? this._Files.ToList() : null;
}