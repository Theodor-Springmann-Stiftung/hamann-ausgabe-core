namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaXMLReader.Interfaces;

public interface IXMLService {
    public IXMLRoot? GetRoot(string name);
    public List<IXMLRoot>? GetRootsList();
    public Dictionary<string, IXMLRoot>? GetRootsDictionary();
    public List<XMLRootDocument>? ProbeFile(XDocument document, ModelStateDictionary ModelState);
    public Dictionary<string, FileList?>? GetUsedDictionary();
    public XElement? MergeUsedDocuments(ModelStateDictionary ModelState);
    public void Use(XMLRootDocument doc);
    public void AutoUse(string prefix);
    public void AutoUse(FileList filelist);
    public Dictionary<string, FileList?>? GetInProduction();
    public void UnUse(string prefix);
    public void UnUseProduction();
    public void SetInProduction();
    public void SetInProduction(XDocument document);
    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader);
}