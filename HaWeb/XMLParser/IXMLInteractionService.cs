namespace HaWeb.XMLParser;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaWeb.Models;
using HaDocument.Interfaces;
using HaDocument.Models;
using HaXMLReader.Interfaces;
using Microsoft.Extensions.FileProviders;

public interface IXMLInteractionService {
    public XElement? TryCreate();
    public bool GetValidState();
    public void Collect(List<IFileInfo> Files);
    public Dictionary<string, FileList?>? GetLoaded();
    public IXMLRoot? GetRoot(string name);
    public List<IXMLRoot>? GetRootsList();
    public void CreateSearchables(XDocument document);
    public List<FileModel>? GetManagedFiles();
    public Dictionary<string, SyntaxCheckModel>? Test();
    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? SearchCollection(string collection, string searchword, IReaderService reader, ILibrary? lib);
    public List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? GetPreviews(List<(string, List<Marginal>)> places, IReaderService reader, ILibrary lib);
}