using System.Text;
using Microsoft.Extensions.FileProviders;

namespace HaWeb.Models;

public class FileModel {
    public string FileName { get; private set; }
    public IFileInfo File { get; private set; }

    // This affects only repo files
    public bool IsValid { get; private set; } = false;
    public List<XMLRootDocument>? Content { get; set; }
    public List<(string, string?)>? Fields { get; set; }
    public string? Prefix { get; set; }

    private StringBuilder? _log;

    public FileModel(string name, IFileInfo file) {
        FileName = name;
        File = file;
    }

    public string? GetLog() {
        if (_log == null) return null;
        return _log.ToString();
    }

    public void Log(string msg) {
        if (_log == null) _log = new StringBuilder();
        _log.AppendLine(msg);
    }

    public void ResetLog() {
        if (_log != null) _log.Clear();
    }

    public void Validate() { 
        IsValid = true; 
    }

    public DateTime GetLastModified() {
        return File.LastModified.ToLocalTime().DateTime;
    }
}