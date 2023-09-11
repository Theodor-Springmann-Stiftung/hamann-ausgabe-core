using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HaDocument.Interfaces;
using HaDocument.Models;
using HaWeb.FileHelpers;
using HaWeb.Models;
using HaWeb.SearchHelpers;
using HaWeb.XMLParser;
using HaWeb.XMLTests;
using HaXMLReader.Interfaces;
using Microsoft.Extensions.FileProviders;

// Conditions for Successful create
// All types there
// Merging Success
// Saving Success
// Loading Success

// Startup (BEFORE IXMLFileProvider, After IHaDocumentWrapper)
public class XMLInteractionService : IXMLInteractionService {
    private readonly IXMLTestService _testService;
    private readonly long _fileSizeLimit;
    private readonly string[] _allowedExtensions = { ".xml" };
    private readonly static XmlReaderSettings _xmlSettings = new XmlReaderSettings() {
        CloseInput = true,
        CheckCharacters = false,
        ConformanceLevel = ConformanceLevel.Fragment,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = false
    };

    private Dictionary<string, IXMLRoot>? _RootDefs;
    private Dictionary<string, IXMLCollection>? _CollectionDefs;
    private Dictionary<string, ItemsCollection>? _Collection;

    public event EventHandler<Dictionary<string, SyntaxCheckModel>?> SyntaxCheck;

    private XMLParsingState? _State;

    private Dictionary<string, SyntaxCheckModel>? _SCCache;
    
    public XMLInteractionService(IConfiguration config, IXMLTestService testService) {
        _testService = testService;
        _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
        var roottypes = _GetAllTypesThatImplementInterface<IXMLRoot>().ToList();
        roottypes.ForEach( x => {
            if (this._RootDefs == null) this._RootDefs = new Dictionary<string, IXMLRoot>();
            var instance = (IXMLRoot)Activator.CreateInstance(x)!;
            if (instance != null) this._RootDefs.Add(instance.Prefix, instance);
        });

        var collectiontypes = _GetAllTypesThatImplementInterface<IXMLCollection>().ToList();
        collectiontypes.ForEach( x => {
            if (this._CollectionDefs == null) this._CollectionDefs = new Dictionary<string, IXMLCollection>();
            var instance = (IXMLCollection)Activator.CreateInstance(x)!;
            if (instance != null && instance.IsGlobal()) this._CollectionDefs.Add(instance.Key, instance);
        });

        if (_RootDefs == null || !_RootDefs.Any())
            throw new Exception("No classes for upload endpoints were found!");

        if (_CollectionDefs == null || !_CollectionDefs.Any())
            throw new Exception("No classes for object collection were found!");
    }

    // Getters and Setters
    public XMLParsingState? GetState() => this._State;

    public void SetState(XMLParsingState? state) => this._State = state;

    public Dictionary<string, IXMLRoot>? GetRootDefs() => this._RootDefs;

    public Dictionary<string, SyntaxCheckModel>? GetSCCache() => this._SCCache;

    public void SetSCCache(Dictionary<string, SyntaxCheckModel>? cache) => this._SCCache = cache;

    // Functions
    public XMLParsingState? Collect(List<IFileInfo> files, Dictionary<string, IXMLRoot>? rootDefs) {
        if (files == null || !files.Any() || rootDefs == null || !rootDefs.Any()) return null;
        var _state = new XMLParsingState() {
            ValidState = true
        };
        foreach (var f in files) {
            var m = _CreateFileModel(f, null);
            _state.ManagedFiles!.Add(m);
            // 1. Open File for Reading
            try {
                using (Stream file = f.CreateReadStream()) {
                    // 2. Some security checks, if file empty, wrong start, wrong extension, too big
                    if (!XMLFileHelpers.ProcessFile(file, f.Name, m.Log, _allowedExtensions, _fileSizeLimit))  continue;
                }
            } catch {
                m.Log( "Datei konnte nicht geöffnet werden.");
                continue;
            }

            // 3. Check validity of XML
            try {
                using (var xmlreader = XmlReader.Create(f.CreateReadStream(), _xmlSettings)) {
                    var doc = XDocument.Load(xmlreader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

                    // 4. Check if opus-Document
                    // TODO: Unter der HOOD werden in ProbeFiles noch eigene Files gebaut!
                    var docs = _ProbeFile(doc, m, rootDefs);
                    if (docs == null || !docs.Any()) continue;

                    // Success! File can be recognized and parsed.
                    m.Validate();
                    foreach (var d in docs) {
                        if (!_state.Loaded!.ContainsKey(d.Prefix)) _state.Loaded.Add(d.Prefix, new FileList(d.XMLRoot));
                        _state.Loaded[d.Prefix]!.Add(d);
                    }
                }
            } catch (Exception ex) {
                m.Log($"Ungültiges XML: {ex.Message}");
                continue;
            }
        }

        foreach (var f in _state.ManagedFiles!) {
            if (!f.IsValid) {
                _state.ValidState = false;
                break;
            }
        }
        return _state;
    }

    // Every caller shoud ask the cache above first
    public Dictionary<string, SyntaxCheckModel>? Test(XMLParsingState? state, string gitcommit) {
        if (state == null || state.Loaded == null) return null;
        // TODO: Speed up this, move it into a background task:
        var sw = new Stopwatch();
        sw.Start();
        var res = state.Loaded?.SelectMany(x => x.Value?.GetFileList()?.Select(x => x.File)).Distinct().Select(x => x.FileName);
        var ret = _testService.Test(state.Loaded, res.ToDictionary(x => x, y => new SyntaxCheckModel(y, gitcommit)));
        if (ret != null)
            foreach (var r in ret) {
                r.Value.SortErrors();
            }
        sw.Stop();
        Console.WriteLine("Syntaxcheck " + sw.ElapsedMilliseconds.ToString() + " ms");
        OnSyntaxCheck(ret);
        return ret;
    }

    public XElement? TryCreate(XMLParsingState state) {
        if (state.Loaded == null || !state.Loaded.Any() || _RootDefs == null || !_RootDefs.Any() || !state.ValidState) return null;
        var opus = new XElement("opus");
        // TODO: Workaround for bug in HaDocument: roots have to be added in a specific order
        var used = state.Loaded.OrderByDescending(x => x.Key);
        foreach (var category in used) {
            if (category.Value == null || category.Value.GetFileList() == null || !category.Value.GetFileList()!.Any()) {
                return null;
            }
            var documents = category.Value.GetFileList();
            foreach (var document in documents!) {
                document.XMLRoot.MergeIntoFile(opus, document);
            }
        }
        return opus;
    }

    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? GetPreviews(List<(string, List<Marginal>)> places, IReaderService reader, ILibrary lib) {
         if (!_Collection.ContainsKey("letters")) return null;
        var searchableObjects = _Collection["letters"].Items;
        var res = new ConcurrentBag<(string Index, List<(string Page, string Line, string preview, string identifier)> Results)>();

        Parallel.ForEach(places, (obj) => {
            var text = searchableObjects[obj.Item1];
            if (text == null || text.SearchText == null || obj.Item2 == null || !obj.Item2.Any()) return;
            var state = new SearchState(String.Empty, false, lib);
            var rd = reader.RequestStringReader(text.SearchText);
            var parser = new HaWeb.HTMLParser.LineXMLHelper<SearchState>(state, rd, new StringBuilder(), null, null, null, null, null);
            rd.Read();

            res.Add((
                obj.Item1,
                obj.Item2.Select(x => (
                    x.Page,
                    x.Line,
                    parser.Lines != null ?
                        parser.Lines
                        .Where(y => y.Page == x.Page && y.Line == x.Line)
                        .Select(y => y.Text)
                        .FirstOrDefault(string.Empty)
                        : string.Empty,
                    String.Empty
                ) ).ToList()
            ));
        });

        return res.ToList();
    }

     public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader, ILibrary lib) {
        if (!_Collection.ContainsKey(collection)) return null;
        var searchableObjects = _Collection[collection].Items;
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

    public void CreateSearchables(XDocument document) {
        if (document == null || _RootDefs == null) return;
        int numProcs = Environment.ProcessorCount;
        int concurrencyLevel = numProcs * 2;
        int startingSize = 2909;
        int startingSizeAllCollections = 23;
        var ret = new ConcurrentDictionary<string, ItemsCollection>(concurrencyLevel, startingSizeAllCollections);

        if (_CollectionDefs != null)
            Parallel.ForEach(_CollectionDefs, (coll) => { 
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
        _Collection = ret.ToDictionary(x => x.Key, y => y.Value);
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>() {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }

    private List<XMLRootDocument>? _ProbeFile(XDocument document, FileModel file, Dictionary<string, IXMLRoot>? rootDefs) {
        if (document.Root!.Name != "opus") {
            file.Log("Ein gültiges Dokument muss mit <opus> beginnen.");
            return null;
        }

        List<XMLRootDocument>? res = null;
        if (document.Root != null && rootDefs != null) {
            foreach (var (_, root) in rootDefs) {
                var elements = root.IsTypeOf(document.Root);
                if (elements != null &&  elements.Any())
                    foreach (var elem in elements) {
                        if (res == null) res = new List<XMLRootDocument>();
                        res.Add(_createXMLRootDocument(root, elem, file));
                    }
            }
        }
        if (res == null) file.Log("Dokumenten-Typ nicht erkannt.");
        return res;
    }

    private XMLRootDocument _createXMLRootDocument(IXMLRoot Root, XElement element, FileModel file) {
        var doc = new XMLRootDocument(Root, Root.Prefix, Root.GenerateIdentificationString(element), element, file);
        doc.Fields = Root.GenerateFields(doc);
        return doc;
    }

    private FileModel _CreateFileModel(IFileInfo file, string? message) {
        var m = new FileModel(file.Name, file);
        if (!String.IsNullOrWhiteSpace(message)) {
            m.Log(message);
        }
        return m;
    }

    protected virtual void OnSyntaxCheck(Dictionary<string, SyntaxCheckModel>? state) {
        EventHandler<Dictionary<string, SyntaxCheckModel>?> eh = SyntaxCheck;
        eh?.Invoke(this, state);
    }
}