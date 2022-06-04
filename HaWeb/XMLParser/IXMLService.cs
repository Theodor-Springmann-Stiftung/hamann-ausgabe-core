namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLService {
    public IXMLRoot? GetRoot(string name);
    public List<IXMLRoot>? GetRoots();
    public Task<List<XMLRootDocument>?> ProbeHamannFile(XDocument document, ModelStateDictionary ModelState);
    public Task UpdateAvailableFiles(XMLRootDocument doc, string basefilepath, ModelStateDictionary ModelState);
    public Dictionary<string, List<XMLRootDocument>> GetUsed();
}