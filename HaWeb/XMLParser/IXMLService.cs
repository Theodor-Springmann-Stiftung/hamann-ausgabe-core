namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;

public interface IXMLService {
    public IXMLRoot? GetRoot(string name);
    public List<IXMLRoot>? GetRootsList();
    public Dictionary<string, IXMLRoot>? GetRootsDictionary();
    public Task<List<XMLRootDocument>?> ProbeHamannFile(XDocument document, ModelStateDictionary ModelState);
    public Dictionary<string, FileList?>? GetUsedDictionary();
    public void Use(XMLRootDocument doc);
    public void AutoUse(string prefix);
    public void AutoUse(FileList filelist);
}