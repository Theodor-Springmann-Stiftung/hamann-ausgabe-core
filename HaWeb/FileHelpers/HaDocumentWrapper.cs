namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class HaDocumentWrapper : IHaDocumentWrappper {
    public ILibrary Library;

    public HaDocumentWrapper() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions());
    }

    public ILibrary SetLibrary() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions());
        return Library;
    }

    public ILibrary? SetLibrary(string filepath, ModelStateDictionary ModelState) {
        try 
        {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath });
        }
        catch (Exception ex) {
            ModelState.AddModelError("Error:", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            return null;
        }
        return Library;
    }

    public ILibrary GetLibrary() {
        return Library;
    }
}