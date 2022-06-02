namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IXMLService {
    public List<IXMLRoot>? GetRoots();
    public List<XMLRootDocument>? ProbeHamannFile(XDocument document, ModelStateDictionary ModelState);
}