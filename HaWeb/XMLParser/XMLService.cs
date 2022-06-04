namespace HaWeb.XMLParser;
using HaWeb.Settings.XMLRoots;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using Microsoft.FeatureManagement;

public class XMLService : IXMLService {
    private Dictionary<string, List<XMLRootDocument>?>? _availableFilesObj;
    private Dictionary<string, List<XMLRootDocument>?>? _availableFiles { 
        get {
            if (_availableFilesObj == null)  {
                _availableFilesObj = _GetAvailableFiles();
                AutoDetermineUsed();
            }
            return _availableFilesObj;
    } }
    
    private IFileProvider _fileProvider;
    private IFeatureManager _featureManager;
    private Dictionary<string, IXMLRoot>? _Roots;

    public bool UploadEnabled = false;
    public bool UpdateEnabled = false;

    public XMLService(IFileProvider provider, IFeatureManager featureManager) {
        _fileProvider = provider;
        _featureManager = featureManager;
        if (provider == null)
            throw new Exception("To Upload Files you need a FileProvider");

        // Getting all classes which implement IXMLRoot for possible upload endpoints
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
        _Roots.TryGetValue(name, out var root);
        return root;
    }

    public List<IXMLRoot>? GetRoots() => this._Roots == null ? null : this._Roots.Values.ToList();

    public async Task<List<XMLRootDocument>?> ProbeHamannFile(XDocument document, ModelStateDictionary ModelState) {
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

    public void UpdateAvailableFiles() {
        _availableFilesObj = _GetAvailableFiles();
    }

    public void UpdateAvailableFiles(string prefix) {
        if (_availableFilesObj == null)  {
            UpdateAvailableFiles();
            return;
        }
        if (_availableFilesObj.ContainsKey(prefix))
            _availableFilesObj.Remove(prefix);
        if (_fileProvider.GetDirectoryContents(prefix).Exists) {
            _availableFilesObj.Add(prefix, _GetAvailableFiles(prefix));
        }

        AutoDetermineUsed(prefix);
    }

    public List<XMLRootDocument>? GetAvailableFiles(string prefix) {
        if (_Roots == null || _availableFiles == null) return null;
        if(!_Roots.ContainsKey(prefix) || !_availableFiles.ContainsKey(prefix)) return null;

        return _availableFiles[prefix];
    }

    public async Task UpdateAvailableFiles(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState) {
        await _setEnabled();
        if (!UploadEnabled) {
            ModelState.AddModelError("Error", "The uploading of files is deactivated");
            return;
        }

        await _Save(doc, basefilepath, ModelState);
        if (!ModelState.IsValid) return;

        UpdateAvailableFiles(doc.Prefix);
    }

    private async Task _Save(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState) {
        var type = doc.Prefix;
        var directory = Path.Combine(basefilepath, type);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, doc.FileName);
        try {
            using (var targetStream = System.IO.File.Create(path))
                await doc.Save(targetStream, ModelState);
        }
        catch (Exception ex) {
            ModelState.AddModelError("Error",  "Speichern der Datei fehlgeschlagen: " + ex.Message);
            return;
        }
        
        var info = _fileProvider.GetFileInfo(Path.Combine(doc.Prefix, doc.FileName));
        if (info == null) {
            ModelState.AddModelError("Error", "Auf die neu erstellte Dtaei konnte nicht zugegriffen werden");
            return;
        }
        doc.SetFile(info);

        UpdateAvailableFiles(type);
    }


    private XMLRootDocument _createXMLRootDocument(IXMLRoot Root, XElement element) {
        var doc = new XMLRootDocument(this, Root.Prefix, Root.GenerateIdentificationString(element), element);
        doc.Fields = Root.GenerateFields(doc);
        return doc;
    }

    private IEnumerable<Type> _GetAllTypesThatImplementInterface<T>()
    {
        return System.Reflection.Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface);
    }

    private async Task _setEnabled() {
        if (await _featureManager.IsEnabledAsync(HaWeb.Features.UploadService))
            UploadEnabled = true;
        if (await _featureManager.IsEnabledAsync(HaWeb.Features.UpdateService))
            UpdateEnabled = true;
    }

    private Dictionary<string, List<XMLRootDocument>?>? _GetAvailableFiles() {
        if (_Roots == null) return null;
        Dictionary<string, List<XMLRootDocument>?>? res = null;
        var dirs = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.IsDirectory);
        foreach(var dir in dirs) {
            if(_Roots.ContainsKey(dir.Name)) {
                if (res == null) res = new Dictionary<string, List<XMLRootDocument>?>();
                res.Add(dir.Name, _GetAvailableFiles(dir.Name));                
            }
        }
        return res;
    }

    private List<XMLRootDocument>? _GetAvailableFiles(string prefix) {
        List<XMLRootDocument>? res = null;
        var files = _fileProvider.GetDirectoryContents(prefix).Where(x => !x.IsDirectory && x.Name.StartsWith(prefix) && x.Name.EndsWith(".xml"));
        foreach (var file in files) {
            if (res == null) res = new List<XMLRootDocument>();
            res.Add(new XMLRootDocument(this, file));
        }
        return res;
    }

    public void AutoDetermineUsed() {
        if (_availableFilesObj == null) return;
        foreach (var (prefix, _) in _availableFilesObj)
            AutoDetermineUsed(prefix);
    }

    public void AutoDetermineUsed(string prefix) {
        if (_Roots == null || _availableFilesObj == null) return;
        _Roots.TryGetValue(prefix, out var root);
        _availableFilesObj.TryGetValue(prefix, out var files);
        if (files == null || root == null) return;

        //TODO: Item1
        var lookup = files.ToLookup(x => x.IdentificationString.Item2);
        foreach (var idstring in lookup) {
            var ordered = idstring.OrderBy(x => x.Date);
            ordered.Last().SetUsed(true);
            ordered.Take(ordered.Count() - 1).ToList().ForEach(x => x.SetUsed(false));
        }
    }

    public Dictionary<string, List<XMLRootDocument>>? GetUsed() {
        if (_availableFiles == null) return null;
        return _availableFiles
            .Where(x => x.Value != null)
            .Select(x => x.Value!.Where(x => x.Used).ToList())
            .Where(x => x.Any())
            .ToDictionary(x => x.First().Prefix);
    }
}