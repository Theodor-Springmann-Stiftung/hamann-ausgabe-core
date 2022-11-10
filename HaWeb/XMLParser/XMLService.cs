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
using HaDocument.Interfaces;

public class XMLService : IXMLService {
    private Dictionary<string, FileList?>? _Used;
    private Dictionary<string, IXMLRoot>? _Roots;
    private Dictionary<string, IXMLCollection>? _Collections;

    private Stack<Dictionary<string, FileList?>>? _InProduction;

    private Dictionary<string, ItemsCollection> _collectedProduction;
    private Dictionary<string, ItemsCollection> _collectedUsed;

    public XMLService() {
        // Getting all classes which implement IXMLRoot for possible document endpoints
        var roottypes = _GetAllTypesThatImplementInterface<IXMLRoot>().ToList();
        roottypes.ForEach( x => {
            if (this._Roots == null) this._Roots = new Dictionary<string, IXMLRoot>();
            var instance = (IXMLRoot)Activator.CreateInstance(x)!;
            if (instance != null) this._Roots.Add(instance.Prefix, instance);
        });

        var collectiontypes = _GetAllTypesThatImplementInterface<IXMLCollection>().ToList();
        collectiontypes.ForEach( x => {
            if (this._Collections == null) this._Collections = new Dictionary<string, IXMLCollection>();
            var instance = (IXMLCollection)Activator.CreateInstance(x)!;
            if (instance != null && instance.IsGlobal()) this._Collections.Add(instance.Key, instance);
        });

        if (_Roots == null || !_Roots.Any())
            throw new Exception("No classes for upload endpoints were found!");

        if (_Collections == null || !_Collections.Any())
            throw new Exception("No classes for object collection were found!");
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
        int numProcs = Environment.ProcessorCount;
        int concurrencyLevel = numProcs * 2;
        int startingSize = 2909;
        int startingSizeAllCollections = 23;
        var ret = new ConcurrentDictionary<string, ItemsCollection>(concurrencyLevel, startingSizeAllCollections);

        if (_Collections != null)
            Parallel.ForEach(_Collections, (coll) => { 
                var elem = coll.Value.xPath.Aggregate(new List<XElement>(), (x, y) =>  { x.AddRange(document.XPathSelectElements(y).ToList()); return x; } );
                if (elem != null && elem.Any()) {
                    var items = new ConcurrentDictionary<string, CollectedItem>(concurrencyLevel, startingSize);
                    foreach (var e in elem) {
                        var k = coll.Value.GenerateKey(e);
                        if (k != null) {
                            var searchtext = coll.Value.Searchable ? 
                                StringHelpers.NormalizeWhiteSpace(e.ToString(), ' ', false) : 
                                null;
                            var datafileds = coll.Value.GenerateDataFields != null ? 
                                coll.Value.GenerateDataFields(e) :
                                null;
                            items[k] = new CollectedItem(k, e, coll.Value, datafileds, searchtext);
                        }
                    }
                    if (items.Any()) {
                        if (!ret.ContainsKey(coll.Key)) 
                            ret[coll.Key] = new ItemsCollection(coll.Key, coll.Value);
                        foreach (var item in items) 
                            ret[coll.Key].Items.Add(item.Key, item.Value);
                    }
                }
            });

        if (ret.Any()) {
            Parallel.ForEach(ret, (collection) => {
                collection.Value.GenerateGroupings();
            });
        }
        _collectedProduction = ret.ToDictionary(x => x.Key, y => y.Value);
    }

     public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader, ILibrary? lib = null) {
        if (!_collectedProduction.ContainsKey(collection)) return null;
        var searchableObjects = _collectedProduction[collection].Items;
        var res = new ConcurrentBag<(string Index, List<(string Page, string Line, string preview, string identifier)> Results)>();
        var sw = StringHelpers.NormalizeWhiteSpace(searchword.Trim());

        // Non Parallel:
        // foreach (var obj in searchableObjects) {
        //     if (obj.Value.SearchText != null) {
        //         var state = new SearchState(sw, false, lib);
        //         var rd = reader.RequestStringReader(obj.Value.SearchText);
        //         var parser = new HaWeb.HTMLParser.LineXMLHelper<SearchState>(state, rd, new StringBuilder(), SearchRules.OTagRules, null, null, SearchRules.TextRules, SearchRules.WhitespaceRules);
        //         rd.Read();
        //         if (state.Results != null)
        //             res.Add((
        //                 obj.Value.Index,
        //                 state.Results.Select(x => (
        //                     x.Page,
        //                     x.Line,
        //                     parser.Lines != null ?
        //                         parser.Lines
        //                         .Where(y => y.Page == x.Page && y.Line == x.Line)
        //                         .Select(x => x.Text)
        //                         .FirstOrDefault(string.Empty)
        //                         : "",
        //                     x.Identifier
        //                 )).ToList()));
        //     }
        // }

        Parallel.ForEach(searchableObjects, (obj) => {
            if (obj.Value.SearchText != null) {
                var state = new SearchState(sw, false, lib);
                var rd = reader.RequestStringReader(obj.Value.SearchText);
                var parser = new HaWeb.HTMLParser.LineXMLHelper<SearchState>(state, rd, new StringBuilder(), SearchRules.OTagRules, SearchRules.OTagRules, null, SearchRules.TextRules, SearchRules.WhitespaceRules);
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
                                : "",
                            x.Identifier
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