namespace HaWeb.Models;

public class FileModel {
    public string FileName { get; private set; }
    public string Prefix { get; private set; }
    public DateTime LastModified { get; private set; }
    public bool IsUsed { get; private set; }
    public bool InProduction { get; private set; }
    public List<(string, string?)>? Fields { get; set; }

    public FileModel(string name, string prefix, DateTime lastModified, bool isUsed, bool inProduction) {
        FileName = name;
        IsUsed = isUsed;
        LastModified = lastModified;
        InProduction = inProduction;
    }
}