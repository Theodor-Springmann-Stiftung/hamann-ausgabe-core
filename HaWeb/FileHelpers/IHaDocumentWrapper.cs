namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IHaDocumentWrappper {
    public ILibrary ResetLibrary();
    public ILibrary? SetLibrary(string filepath, ModelStateDictionary ModelState);
    public ILibrary GetLibrary();
}