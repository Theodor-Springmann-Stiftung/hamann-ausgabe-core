namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;

public interface IHaDocumentWrappper {
    public ILibrary SetLibrary();
    public ILibrary GetLibrary();
}