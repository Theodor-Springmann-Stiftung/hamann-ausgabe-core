namespace HaWeb.Models;
using HaWeb.XMLParser;

public class UploadViewModel {
    public string ActiveTitle { get; private set; }
    public string? Prefix { get; private set; }
    public List<IXMLRoot>? AvailableRoots { get; private set; }
    public FileList? AvailableFiles { get; private set; }
    public Dictionary<string, FileList>? UsedFiles { get; private set; }


    public UploadViewModel(string title, string? prefix, List<IXMLRoot>? roots, FileList? availableFiles, Dictionary<string, FileList>? usedFiles) {
        Prefix = prefix;
        ActiveTitle = title;
        AvailableRoots = roots;
        AvailableFiles = availableFiles;
        UsedFiles = usedFiles;
    }
}