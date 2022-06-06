namespace HaWeb.Models;
using HaWeb.XMLParser;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Rendering;

public class UploadViewModel {
    public string ActiveTitle { get; private set; }
    public string? Prefix { get; private set; }
    public List<IXMLRoot>? AvailableRoots { get; private set; }
    public List<FileModel>? AvailableFiles { get; set; }
    public Dictionary<string, List<FileModel>?>? UsedFiles { get; private set; }
    public Dictionary<string, List<FileModel>?>? ProductionFiles { get; set; }

    public List<(string, DateTime)>? HamannFiles { get; set; }

    public UploadViewModel(string title, string? prefix, List<IXMLRoot>? roots, Dictionary<string, List<FileModel>?>? usedFiles) {
        Prefix = prefix;
        ActiveTitle = title;
        AvailableRoots = roots;
        UsedFiles = usedFiles;
    }
}