namespace HaWeb.Models;
using HaWeb.XMLParser;

public class UploadViewModel {
    public List<IXMLRoot>? AvailableRoots { get; set; }
    public List<XMLRootDocument>? AvailableFiles { get; set; }
    public Dictionary<string, List<XMLRootDocument>>? UsedFiles { get; set; }
}