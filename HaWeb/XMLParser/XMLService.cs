namespace HaWeb.XMLParser;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaWeb.SearchHelpers;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text;
using HaXMLReader.Interfaces;

public class XMLService : IXMLService {
    private Dictionary<string, FileList?>? _Used;
    private Dictionary<string, IXMLRoot>? _Roots;

    private Stack<Dictionary<string, FileList?>>? _InProduction;

    private Dictionary<string, Dictionary<string, CollectedItem>> _collectedProduction;
    private Dictionary<string, Dictionary<string, CollectedItem>> _collectedUsed;

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

    public Dictionary<string, FileList?>? GetInProduction() { 
        if (_InProduction == null) return null;
        return this._InProduction.Peek();
    }

    public void SetInProduction() {
        if (_Used == null) return;
        var inProduction = new Dictionary<string, FileList?>();
        foreach (var category in _Used) {
            if (category.Value == null || category.Value.GetFileList() == null || !category.Value.GetFileList()!.Any()) 
                return;
            inProduction.Add(category.Key, category.Value);
        }

        if(_InProduction == null) _InProduction = new Stack<Dictionary<string, FileList?>>();
        _InProduction.Push(inProduction);
    }

    public void SetInProduction(XDocument document) {
        if (document == null || _Roots == null) return;
        var ret = new ConcurrentDictionary<string, ConcurrentDictionary<string, CollectedItem>>();
        Parallel.ForEach(_Roots, (root) => {
            if (root.Value.XPathCollection != null)
                foreach (var coll in root.Value.XPathCollection) {
                    var elem = document.XPathSelectElements(coll.xPath);
                    if (elem != null && elem.Any()) {
                        if (!ret.ContainsKey(coll.Key))
                            ret[coll.Key] = new ConcurrentDictionary<string, CollectedItem>();
                        foreach(var e in elem) {
                            var k = coll.KeyFunc(e);
                            if (k != null) {
                                var searchtext = coll.Searchable ? 
                                    StringHelpers.NormalizeWhiteSpace(e.ToString(), ' ', false) : 
                                    null;
                                ret[coll.Key][k] = new CollectedItem(k, e, root.Value, coll.Key, searchtext);
                            }
                        }
                    }
                }
        });       
        _collectedProduction = ret.ToDictionary(x => x.Key, y => y.Value.ToDictionary(z => z.Key, f => f.Value, null), null);
    }

     public List<(string Index, List<(string Page, string Line, string Preview)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader) {
        if (!_collectedProduction.ContainsKey(collection)) return null;
        var searchableObjects = _collectedProduction[collection];
        var res = new ConcurrentBag<(string Index, List<(string Page, string Line, string preview)> Results)>();
        var sw = StringHelpers.NormalizeWhiteSpace(searchword.Trim());
        Parallel.ForEach(searchableObjects, (obj) => {
            if (obj.Value.SearchText != null) {
                var state = new SearchState(sw);
                var rd = reader.RequestStringReader(obj.Value.SearchText);
                var parser = new HaWeb.HTMLParser.LineXMLHelper<SearchState>(state, rd, new StringBuilder(), null, null, null, SearchRules.TextRules, SearchRules.WhitespaceRules);
                rd.Read();
                if (state.Results != null)
                    res.Add((
                        obj.Value.Index,
                        state.Results.Select(x => (
                            x.Page,
                            x.Line,
                            parser.Lines != null ?
                                parser.Lines
                                .Where(y => y.Page == x.Page && y.Line == x.Line)
                                .Select(x => x.Text)
                                .FirstOrDefault(string.Empty)
                                : ""
                        )).ToList()));
            }
        });
        return res.ToList();
    }

    public void UnUseProduction() => this._InProduction = null;

    public List<XMLRootDocument>? ProbeFile(XDocument document, ModelStateDictionary ModelState) {
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
        // TODO: Workaround for bug in HaDocument: roots have to be added in a specific order
        var used = _Used.OrderByDescending(x => x.Key);
        foreach (var category in used) {
            if (category.Value == null || category.Value.GetFileList() == null || !category.Value.GetFileList()!.Any()) {
                ModelState.AddModelError("Error", _Roots![category.Key].Type + " nicht vorhanden.");
                return null;
            }
            var documents = category.Value.GetFileList();
            foreach (var document in documents!) {
                document.XMLRoot.MergeIntoFile(opus, document);
            }
        }

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