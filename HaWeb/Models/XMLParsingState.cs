namespace HaWeb.Models;

public class XMLParsingState : EventArgs {
    internal List<FileModel>? ManagedFiles { get; set; }
    internal Dictionary<string, FileList?>? Loaded { get; set; }
    internal bool ValidState { get; set; }
    
    public XMLParsingState() {
        ManagedFiles = new();
        Loaded = new();
        ValidState = false;
    }
}