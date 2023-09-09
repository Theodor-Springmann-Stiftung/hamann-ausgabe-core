namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaWeb.XMLParser;
using HaWeb.XMLTests;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;

// XMLProvider provides a wrapper around the available XML data on a FILE basis
public class XMLFileProvider : IXMLFileProvider {
    private readonly IHaDocumentWrappper _Lib;
    private readonly IXMLInteractionService _XMLService;

    private IFileProvider _hamannFileProvider;
    private IFileProvider _bareRepositoryFileProvider;
    private IFileProvider _workingTreeFileProvider;

    private string _Branch;

    private List<IFileInfo>? _WorkingTreeFiles;
    private List<IFileInfo>? _HamannFiles;
    
    private static (DateTime PullTime, string Hash)? _GitData;

    // Startup (LAST)
    public XMLFileProvider(IXMLInteractionService xmlservice, IHaDocumentWrappper _lib, IConfiguration config) {
        // TODO: Test Read / Write Access
        _Lib = _lib;
        _XMLService = xmlservice;

        _Branch = config.GetValue<string>("RepositoryBranch");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            _hamannFileProvider = new PhysicalFileProvider(config.GetValue<string>("HamannFileStoreWindows"));
            _bareRepositoryFileProvider = new PhysicalFileProvider(config.GetValue<string>("BareRepositoryPathWindows")); 
            _workingTreeFileProvider = new PhysicalFileProvider(config.GetValue<string>("WorkingTreePathWindows")); 
        } 
        else {
            _hamannFileProvider = new PhysicalFileProvider(config.GetValue<string>("HamannFileStoreLinux"));
            _bareRepositoryFileProvider = new PhysicalFileProvider(config.GetValue<string>("BareRepositoryPathLinux"));
            _workingTreeFileProvider = new PhysicalFileProvider(config.GetValue<string>("WorkingTreePathLinux")); 
        }

        // Create File Lists; Here and in xmlservice, which does preliminary checking
        Scan();
        if (_WorkingTreeFiles != null && _WorkingTreeFiles.Any()) {
            xmlservice.Collect(_WorkingTreeFiles);
        }
        _HamannFiles = _ScanHamannFiles();
        
        // Check if hamann file already is current working tree status
        // -> YES: Load up the file via _lib.SetLibrary();
        if (_IsAlreadyParsed()) {
            _Lib.SetLibrary(_HamannFiles.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }

         // -> NO: Try to create a new file
        var created = xmlservice.TryCreate();
        if (created != null) {
            var file = SaveHamannFile(created, _hamannFileProvider.GetFileInfo("./").PhysicalPath, null);
            if (file != null) {
                _lib.SetLibrary(file, created.Document, null);
                if (_Lib.GetLibrary() != null) return;
            }
        }

        // It failed, so use the last best File:
        else if (_HamannFiles != null && _HamannFiles.Any()) {
            _Lib.SetLibrary(_HamannFiles.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }
        
        // -> There is none? Use Fallback:
        else {
            var options = new HaWeb.Settings.HaDocumentOptions();
                if (_lib.SetLibrary(null, null, null) == null) {
                throw new Exception("Die Fallback Hamann.xml unter " + options.HamannXMLFilePath + " kann nicht geparst werden.");
            }
        }
    }

    public void Reload(IConfiguration config) {
        _Branch = config.GetValue<string>("RepositoryBranch");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            _hamannFileProvider = new PhysicalFileProvider(config.GetValue<string>("HamannFileStoreWindows"));
            _bareRepositoryFileProvider = new PhysicalFileProvider(config.GetValue<string>("BareRepositoryPathWindows")); 
            _workingTreeFileProvider = new PhysicalFileProvider(config.GetValue<string>("WorkingTreePathWindows")); 
        } 
        else {
            _hamannFileProvider = new PhysicalFileProvider(config.GetValue<string>("HamannFileStoreLinux"));
            _bareRepositoryFileProvider = new PhysicalFileProvider(config.GetValue<string>("BareRepositoryPathLinux"));
            _workingTreeFileProvider = new PhysicalFileProvider(config.GetValue<string>("WorkingTreePathLinux")); 
        }

        // Create File Lists; Here and in xmlservice, which does preliminary checking
        Scan();
        if (_WorkingTreeFiles != null && _WorkingTreeFiles.Any()) {
            _XMLService.Collect(_WorkingTreeFiles);
        }
        _HamannFiles = _ScanHamannFiles();
        
        // Check if hamann file already is current working tree status
        // -> YES: Load up the file via _lib.SetLibrary();
        if (_IsAlreadyParsed()) {
            _Lib.SetLibrary(_HamannFiles.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }

         // -> NO: Try to create a new file
        var created = _XMLService.TryCreate();
        if (created != null) {
            var file = SaveHamannFile(created, _hamannFileProvider.GetFileInfo("./").PhysicalPath, null);
            if (file != null) {
                _Lib.SetLibrary(file, created.Document, null);
                if (_Lib.GetLibrary() != null) return;
            }
        }

        // It failed, so use the last best File:
        else if (_HamannFiles != null && _HamannFiles.Any()) {
            _Lib.SetLibrary(_HamannFiles.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }
        
        // -> There is none? Use Fallback:
        else {
            var options = new HaWeb.Settings.HaDocumentOptions();
                if (_Lib.SetLibrary(null, null, null) == null) {
                throw new Exception("Die Fallback Hamann.xml unter " + options.HamannXMLFilePath + " kann nicht geparst werden.");
            }
        }
    }

    // Getters and Setters
    public List<IFileInfo>? GetWorkingTreeFiles() => _WorkingTreeFiles;

    public (DateTime PullTime, string Hash)? GetGitData() => _GitData;

    public List<IFileInfo>? GetHamannFiles() => this._HamannFiles;

    // Functions
    public void DeleteHamannFile(string filename) {
        if (_HamannFiles == null) return;
        var files = _HamannFiles.Where(x => x.Name == filename);
        foreach (var file in files) {
            File.Delete(file.PhysicalPath);
        }
        _HamannFiles.RemoveAll(x => x.Name == filename);
    }

    public void Scan() {
        _WorkingTreeFiles = _ScanWorkingTreeFiles();
        _GitData = _ScanGitData();
    }

    public IFileInfo? SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary? ModelState) {
        if (!_GitData.HasValue) return null;
        var filename = "hamann_" + _GitData.Value.PullTime.Year + "-" + _GitData.Value.PullTime.Month + "-" + _GitData.Value.PullTime.Day + "_" + _GitData.Value.PullTime.Hour + "-" + _GitData.Value.PullTime.Minute + "." + _GitData.Value.Hash.Substring(0,7) + ".xml";
        var path = Path.Combine(basefilepath, filename);

        try {
            if (!Directory.Exists(basefilepath))
                Directory.CreateDirectory(basefilepath);
            using (var targetStream = System.IO.File.Create(path))
                element.Save(targetStream, SaveOptions.DisableFormatting);
        } catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Die Datei konnte nicht gespeichert werden: " + ex.Message);
            return null;
        }

        var info = _hamannFileProvider.GetFileInfo(filename);
        if (info == null) {
            if (ModelState != null) ModelState.AddModelError("Error", "Auf die neu erstellte Datei konnte nicht zugegriffen werden.");
            return null;
        }

        if (_HamannFiles == null) _HamannFiles = new List<IFileInfo>();
        _HamannFiles.RemoveAll(x => x.Name == info.Name);
        _HamannFiles.Add(info);
        return info;
    }

    public bool HasChanged() {
        if (!_GitData.HasValue) return true;
        var current = _ScanGitData();
        if (current.Item2 != _GitData.Value.Hash) {
            _GitData = current;
            return true;
        }
        return false;
    }

    private (DateTime, string) _ScanGitData() {
        var head = _bareRepositoryFileProvider.GetFileInfo("refs/heads/" + _Branch);
        return (head.LastModified.DateTime, File.ReadAllText(head.PhysicalPath));
    }

    private void _RegisterChangeCallbacks() {
        var cT = _bareRepositoryFileProvider.Watch("refs/heads/" + _Branch);
    }

    // Gets all XML Files
    private List<IFileInfo>? _ScanWorkingTreeFiles() {
        var files = _workingTreeFileProvider.GetDirectoryContents(string.Empty)!.Where(x => !x.IsDirectory && x.Name.EndsWith(".xml"))!.ToList();
        return files;
    }

    private List<IFileInfo>? _ScanHamannFiles() {
        var files = _hamannFileProvider.GetDirectoryContents(string.Empty).Where(x => !x.IsDirectory && x.Name.StartsWith("hamann") && x.Name.EndsWith(".xml"));
        if (files == null || !files.Any()) return null;
        return files.OrderByDescending(x => x.LastModified).ToList();
    }

    private string? _GetHashFromHamannFilename(string filename) {
        var s = filename.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (s.Length != 3 || s.Last() != "xml" || !s.First().StartsWith("hamann")) return null;
        return s[1];
    }

    private bool _IsAlreadyParsed() {
        if (_HamannFiles == null || !_HamannFiles.Any() || !_GitData.HasValue) return false;
        var fhash = _GetHashFromHamannFilename(_HamannFiles.First().Name);
        var ghash = _GitData.Value.Hash.Substring(0,7);
        return fhash == ghash;
    }
}