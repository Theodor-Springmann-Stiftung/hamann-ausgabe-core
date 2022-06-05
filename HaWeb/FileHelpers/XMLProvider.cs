namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaWeb.XMLParser;
using System.Xml.Linq;

public class XMLProvider : IXMLProvider {
    private IFileProvider _fileProvider;
    private Dictionary<string, FileList?>? _Files;
    private Dictionary<string, IXMLRoot>? _Roots;
    private List<IFileInfo>? _HamannFiles;

    public XMLProvider(IFileProvider provider, IXMLService xmlservice) {
        _fileProvider = provider;
        _Roots = xmlservice.GetRootsDictionary();
        _Files = _ScanFiles();

        if (_Files != null)
            foreach(var category in _Files) 
                if (category.Value != null)
                    xmlservice.AutoUse(category.Value);
    }

    public FileList? GetFiles(string prefix)
        => _Files != null && _Files.ContainsKey(prefix) ? _Files[prefix] : null;

    // Saves a Document as file and adds it to the collection
    public async Task Save(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState) {
        var type = doc.Prefix;
        var directory = Path.Combine(basefilepath, type);
        var path = Path.Combine(directory, doc.FileName);

        try {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (var targetStream = System.IO.File.Create(path))
                await doc.Save(targetStream, ModelState);
        }
        catch (Exception ex) {
            ModelState.AddModelError("Error",  "Speichern der Datei fehlgeschlagen: " + ex.Message);
            return;
        }
        
        var info = _fileProvider.GetFileInfo(Path.Combine(doc.Prefix, doc.FileName));
        if (info == null) {
            ModelState.AddModelError("Error", "Auf die neu erstellte Dtaei konnte nicht zugegriffen werden.");
            return;
        }

        doc.File = info;

        if (_Files == null) _Files = new Dictionary<string, FileList?>();
        if (!_Files.ContainsKey(doc.Prefix)) _Files.Add(doc.Prefix, new FileList(doc.XMLRoot));
        _Files[doc.Prefix]!.Add(doc);
    }

    public async Task<IFileInfo?> SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary ModelState) {
        var date = DateTime.Now;
        var filename = "hamann_" + date.Year + "-" + date.Month + "-" + date.Day + ".xml";
        var directory = Path.Combine(basefilepath, "hamann");
        var path = Path.Combine(directory, filename);

        try {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (var targetStream = System.IO.File.Create(path))
                await element.SaveAsync(targetStream, SaveOptions.DisableFormatting, new CancellationToken());
        }
        catch (Exception ex) {
            ModelState.AddModelError("Error", "Die Datei konnte nicht gespeichert werden: " + ex.Message);
            return null;
        }

        var info = _fileProvider.GetFileInfo(Path.Combine("hamann", filename));
        if (info == null) {
            ModelState.AddModelError("Error", "Auf die neu erstellte Dtaei konnte nicht zugegriffen werden.");
            return null;
        }

        if (_HamannFiles == null) _HamannFiles = new List<IFileInfo>();
        _HamannFiles.Add(info);

        return info;
    }

    private Dictionary<string, FileList?>? _ScanFiles() {
        if (_Roots == null) return null;
        Dictionary<string, FileList?>? res = null;
        var dirs = _fileProvider.GetDirectoryContents(string.Empty).Where(x => x.IsDirectory);
        foreach (var dir in dirs) {
            if (_Roots.ContainsKey(dir.Name)) {
                if (_Files == null) _Files = new Dictionary<string, FileList?>();
                if (res == null) res = new Dictionary<string, FileList?>();
                res.Add(dir.Name, _ScanFiles(dir.Name));
            }
        }
        return res;
    }

    private FileList? _ScanFiles(string prefix) {
        if (_Roots == null) return null;
        FileList? res = null;
        var files = _fileProvider.GetDirectoryContents(prefix).Where(x => !x.IsDirectory && x.Name.StartsWith(prefix) && x.Name.EndsWith(".xml"));
        foreach (var file in files) {
            if (_Roots == null || !_Roots.ContainsKey(prefix))
                throw new Exception("Attempting to read a File from an unrecognized Prefix: " + prefix);
            if (res == null) res = new FileList(_Roots[prefix]);
            res.Add(new XMLRootDocument(_Roots[prefix], file));
        }
        return res;
    }
}