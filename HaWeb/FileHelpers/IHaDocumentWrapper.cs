namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using HaXMLReader.Interfaces;

public interface IHaDocumentWrappper {
    public ILibrary ResetLibrary();
    public ILibrary? SetLibrary(string filepath, ModelStateDictionary ModelState);
    public ILibrary GetLibrary();
    public List<(string Index, List<(string Page, string Line, string Preview)> Results)>? SearchLetters(string searchword, IReaderService reader); 
}