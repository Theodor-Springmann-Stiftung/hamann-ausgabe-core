namespace HaWeb.FileHelpers;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaWeb.XMLParser;
using HaWeb.XMLTests;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;

// XMLProvider provides a wrapper around the available XML data on a FILE basis
public class XMLFileProvider : IXMLFileProvider {
    private readonly IHaDocumentWrappper _Lib;
    private readonly IXMLInteractionService _XMLService;

    private IFileProvider _hamannFileProvider;
    private IFileProvider _bareRepositoryFileProvider;
    private IFileProvider _workingTreeFileProvider;
    
    public event EventHandler<GitState?> FileChange;
    public event EventHandler ConfigReload;
    public event EventHandler<XMLParsingState?> NewState;
    public event EventHandler NewData;

    private string _Branch;
    private string _URL;

    private List<IFileInfo>? _WorkingTreeFiles;
    private List<IFileInfo>? _HamannFiles;
    
    private GitState? _GitState;
    private System.Timers.Timer? _changeTokenTimer;

    // Startup (LAST)
    public XMLFileProvider(IXMLInteractionService xmlservice, IHaDocumentWrappper _lib, IConfiguration config) {
        // TODO: Test Read / Write Access
        _Lib = _lib;
        _XMLService = xmlservice;

        _Branch = config.GetValue<string>("RepositoryBranch");
        _URL = config.GetValue<string>("RepositoryURL");
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
            var state = xmlservice.Collect(_WorkingTreeFiles, xmlservice.GetRootDefs());
            xmlservice.SetState(state);
        }
        _HamannFiles = _ScanHamannFiles();
        
        _RegisterChangeToken();
         // Check if hamann file already is current working tree status
        // -> YES: Load up the file via _lib.SetLibrary();
        if (_IsAlreadyParsed()) {
            _Lib.SetLibrary(_HamannFiles!.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }

         // -> NO: Try to create a new file
        var created = _XMLService.TryCreate(_XMLService.GetState());
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

    public void ParseConfiguration(IConfiguration config) {
        _Branch = config.GetValue<string>("RepositoryBranch");

        Scan();
        // Reset XMLInteractionService
        if (_WorkingTreeFiles != null && _WorkingTreeFiles.Any()) {
            var state = _XMLService.Collect(_WorkingTreeFiles, _XMLService.GetRootDefs());
            _XMLService.SetState(state);
        }
        _HamannFiles = _ScanHamannFiles();
        _XMLService.SetSCCache(null);
        if (_HamannFiles != null && _HamannFiles.Select(x => x.Name).Contains(_Lib.GetActiveFile().Name)) {
            _Lib.SetLibrary(_Lib.GetActiveFile(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }

        // Failed to reload File, reload it all, same procedure as above:
        // Check if hamann file already is current working tree status
        // -> YES: Load up the file via _lib.SetLibrary();
        if (_IsAlreadyParsed()) {
            _Lib.SetLibrary(_HamannFiles!.First(), null, null);
            if (_Lib.GetLibrary() != null) return;
        }

         // -> NO: Try to create a new file
        var created = _XMLService.TryCreate(_XMLService.GetState());
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

    public GitState? GetGitState() => _GitState;

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
        _GitState = _ScanGitData();
    }

    public IFileInfo? SaveHamannFile(XElement element, string basefilepath, ModelStateDictionary? ModelState) {
        if (_GitState == null) return null;
        var filename = "hamann_" + _GitState.PullTime.Year + "-" + _GitState.PullTime.Month + "-" + _GitState.PullTime.Day + "_" + _GitState.PullTime.Hour + "-" + _GitState.PullTime.Minute + "." + _GitState.Commit.Substring(0,7) + ".xml";
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
        if (_GitState == null) return true;
        var current = _ScanGitData();
        if (current != null && !String.Equals(current.Commit, _GitState.Commit)) {
            _GitState = current;
            return true;
        }
        return false;
    }

    private GitState? _ScanGitData() {
        var head = _bareRepositoryFileProvider.GetFileInfo("refs/heads/" + _Branch);
        // TODO: Failsave reading from FIle
        try {
            return new GitState {
                URL = _URL,
                Branch = _Branch,
                PullTime = head.LastModified.ToLocalTime().DateTime,
                Commit = File.ReadAllText(head.PhysicalPath).Trim()
            };
        }
        catch {
            return null;
        }
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
        if (_HamannFiles == null || !_HamannFiles.Any() || _GitState == null) return false;
        var fhash = _GetHashFromHamannFilename(_HamannFiles.First().Name);
        var ghash = _GitState.Commit.Substring(0,7);
        return fhash == ghash;
    }

    private void _RegisterChangeToken() {
        ChangeToken.OnChange(
            () => _bareRepositoryFileProvider.Watch("refs/heads/" + _Branch),
            async (state) => await this._InvokeChanged(state),
            this._ScanGitData()
        );
    }

    private async Task _InvokeChanged(GitState? gitdata) {
        if (_changeTokenTimer != null) return;
        Console.WriteLine("FILECHANGE DETECTED, RELOAD");
        Scan();

        OnFileChange(_ScanGitData());
        // Reset XMLInteractionService
        if (_WorkingTreeFiles != null && _WorkingTreeFiles.Any()) {
            var state = _XMLService.Collect(_WorkingTreeFiles, _XMLService.GetRootDefs());
            _XMLService.SetState(state);
            OnNewState(state);
        }
        
         // -> Try to create a new file
        var created = _XMLService.TryCreate(_XMLService.GetState());
        if (created != null) {
            var file = SaveHamannFile(created, _hamannFileProvider.GetFileInfo("./").PhysicalPath, null);
            if (file != null) {
                var ret = _Lib.SetLibrary(file, created.Document, null);
                if (ret != null) OnNewData();
            }
        }

        _XMLService.SetSCCache(null);
        _GitState = _ScanGitData();
        _changeTokenTimer = new(5000) { AutoReset = false, Enabled = true };
        _changeTokenTimer.Elapsed += this._OnElapsed;
    }

    private void _OnElapsed(Object source, System.Timers.ElapsedEventArgs e) {
        _changeTokenTimer = null;
    }

    protected virtual void OnFileChange(GitState? state) {
        EventHandler<GitState?> eh = FileChange;
        eh?.Invoke(this, state);
    } 

    protected virtual void OnNewState(XMLParsingState? state) {
        EventHandler<XMLParsingState?> eh = NewState;
        eh?.Invoke(this, state);
    }

    protected virtual void OnConfigReload() {
        EventHandler eh = ConfigReload;
        eh?.Invoke(this, System.EventArgs.Empty);
    }

    protected virtual void OnNewData() {
        EventHandler eh = NewData;
        eh?.Invoke(this, System.EventArgs.Empty);
    }
}