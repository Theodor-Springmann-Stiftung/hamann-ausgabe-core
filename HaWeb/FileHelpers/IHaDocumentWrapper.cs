namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaXMLReader.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.Xml.Linq;

public interface IHaDocumentWrappper {
    public ILibrary? SetLibrary(IFileInfo? file, XDocument? doc, ModelStateDictionary? ModelState);
    public ILibrary? GetLibrary();
    public void ParseConfiguration(IConfiguration configuration);
    public int GetStartYear();
    public int GetEndYear();
    public IFileInfo GetActiveFile();
    public List<Person>? GetAvailablePersons();
    public Dictionary<string, Person>? GetPersonsWithLetters();
}