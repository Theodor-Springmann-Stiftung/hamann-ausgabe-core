namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;

public class XMLService : IXMLService {
    private Dictionary<string, FileList?>? _Used;
    private Dictionary<string, IXMLRoot>? _Roots;

    private Dictionary<string, FileList?>? _InProduction;

    public XMLService() {
        // Getting all classes which implement IXMLRoot for possible document endpoints
        var types = _GetAllTypesThatImplementInterface<IXMLRoot>().ToList();
        types.ForEach( x => {
            if (this._Roots == null) this._Roots = new Dictionary<string, IXMLRoot>();
            var instance = (IXMLRoot)Activator.CreateInstance(x)!;
            if (instance != null) this._Roots.Add(instance.Prefix, instance);
        });

        if (_Roots == null || !_Roots.Any())
            throw new Exception("No classes for upload endpoints were found!");
    }

    public IXMLRoot? GetRoot(string name) {
        if (_Roots == null) return null;
        _Roots.TryGetValue(name, out var root);
        return root;
    }

    public List<IXMLRoot>? GetRootsList() => this._Roots == null ? null : this._Roots.Values.ToList();

    public Dictionary<string, IXMLRoot>? GetRootsDictionary() => this._Roots == null ? null : this._Roots;

    public Dictionary<string, FileList?>? GetInProduction() => this._InProduction;

    public void UnUseProduction() => this._InProduction = null;

    public List<XMLRootDocument>? ProbeHamannFile(XDocument document, ModelStateDictionary ModelState) {
        if (document.Root!.Name != "opus") {
            ModelState.AddModelError("Error", "A valid Hamann-Docuemnt must begin with <opus>");
            return null;
        }

        List<XMLRootDocument>? res = null;
        if (document.Root != null && _Roots != null) {
            foreach (var (_, root) in _Roots) {
                var elements = root.IsTypeOf(document.Root);
                if (elements != null &&  elements.Any())
                    foreach (var elem in elements) {
                        if (res == null) res = new List<XMLRootDocument>();
                        res.Add(_createXMLRootDocument(root, elem));
                    }
            }
        }
        if (res == null) ModelState.AddModelError("Error", "Kein zum Hamann-Briefe-Projekt passendes XML gefunden.");
        return res;
    }

    public Dictionary<string, FileList?>? GetUsedDictionary()
        => this._Used;


    // Adds a document and sets it to used
    public void Use(XMLRootDocument doc) {
        if (_Used == null) _Used = new Dictionary<string, FileList?>();
        if (!_Used.ContainsKey(doc.Prefix)) _Used.Add(doc.Prefix, new FileList(doc.XMLRoot));
        _Used[doc.Prefix]!.Add(doc);
    }

    public void UnUse(string prefix) {
        if (_Used != null && _Used.ContainsKey(prefix)) _Used.Remove(prefix);
        return;
    }

    // Performs detection of using on the specified document type
    public void AutoUse(string prefix) {
        if (_Used == null || !_Used.ContainsKey(prefix)) return;
        AutoUse(_Used[prefix]!);
    }

    // Performs detection of using given a list of files
    public void AutoUse(FileList filelist) {
        FileList? res = null;
        var list = filelist.GetFileList();
        var prefix = filelist.XMLRoot.Prefix;

        if (list == null) return;
        if (_Used != null && _Used.ContainsKey(prefix)) _Used.Remove(prefix);

        // TODO: Item1
        var lookup = list.ToLookup(x => x.IdentificationString.Item2);
        foreach (var idstring in lookup) {
            var ordered = idstring.OrderBy(x => x.Date);
            if (res == null) res = new FileList(filelist.XMLRoot);
            Use(ordered.Last());
        }
    }

    public XElement? MergeUsedDocuments(ModelStateDictionary ModelState) {
        if (_Used == null || _Roots == null) {
            ModelState.AddModelError("Error", "Keine Dokumente ausgewählt");
            return null;
        }

        var opus = new XElement("opus");
        var inProduction = new Dictionary<string, FileList?>();
        foreach (var category in _Used) {
            if (category.Value == null || category.Value.GetFileList() == null || !category.Value.GetFileList()!.Any()) {
                ModelState.AddModelError("Error", _Roots![category.Key].Type + " nicht vorhanden.");
                return null;
            }
            inProduction.Add(category.Key, category.Value);
            var documents = category.Value.GetFileList();
            foreach (var document in documents!) {
                document.XMLRoot.MergeIntoFile(opus, document);
            }
        }

        _InProduction = inProduction;
        return opus;
    }

    private XMLRootDocument _createXMLRootDocument(IXMLRoot Root, XElement element) {
        var doc = new XMLRootDocument(Root, Root.Prefix, Root.GenerateIdentificationString(element), element);
        doc.Fields = Root.GenerateFields(doc);
        return doc;
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }
}