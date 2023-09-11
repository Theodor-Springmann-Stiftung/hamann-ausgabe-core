namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaDocument.Interfaces;
using HaDocument.Models;
using HaXMLReader.Interfaces;
using Microsoft.Extensions.FileProviders;

public interface IXMLInteractionService {
    public event EventHandler<Dictionary<string, SyntaxCheckModel>?> SyntaxCheck;
    public XElement? TryCreate(XMLParsingState? state);
    public XMLParsingState? GetState();
    public void SetState(XMLParsingState? state);
    public Dictionary<string, IXMLRoot>? GetRootDefs();
    public Dictionary<string, SyntaxCheckModel>? GetSCCache();
    public void SetSCCache(Dictionary<string, SyntaxCheckModel>? cache);
    public XMLParsingState? Collect(List<IFileInfo> Files, Dictionary<string, IXMLRoot>? rootDefs); // XMLFileProvider 
    public void CreateSearchables(XDocument document); // XMLFileProvider
    public Dictionary<string, SyntaxCheckModel>? Test(XMLParsingState? state, string gitcommit); // XMLFileProvider (optimal), Controller (right now)
    // Controller
    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader, ILibrary? lib);
    // Controller
    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? GetPreviews(List<(string, List<Marginal>)> places, IReaderService reader, ILibrary lib);
}