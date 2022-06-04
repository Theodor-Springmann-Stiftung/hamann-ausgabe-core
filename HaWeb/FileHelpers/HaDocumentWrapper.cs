namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;

public class HaDocumentWrapper : IHaDocumentWrappper {
    public ILibrary Library;

    public HaDocumentWrapper() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions());
    }

    public ILibrary SetLibrary() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions());
        return Library;
    }

    public ILibrary GetLibrary() {
        return Library;
    }
}