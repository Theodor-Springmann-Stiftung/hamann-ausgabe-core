namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;

public class HaDocumentWrapper : IHaDocumentWrappper {
    private ILibrary Library;
    private IXMLProvider _xmlProvider;

    private int _startYear;
    private int _endYear;

    public HaDocumentWrapper(IXMLProvider xmlProvider, IConfiguration configuration) {
        _xmlProvider = xmlProvider;

        _startYear = configuration.GetValue<int>("AvailableStartYear");
        _endYear = configuration.GetValue<int>("AvailableEndYear");
        var filelist = xmlProvider.GetHamannFiles();
        if (filelist != null && filelist.Any()) {
            _AutoLoad(filelist);
        }
        // Use Fallback library
        if (Library == null) 
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { AvailableYearRange = (_startYear, _endYear) });
    }

    public ILibrary ResetLibrary() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { AvailableYearRange = (_startYear, _endYear) });
        return Library;
    }

    public ILibrary? SetLibrary(string filepath, ModelStateDictionary? ModelState = null) {
        try 
        {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath, AvailableYearRange = (_startYear, _endYear) });
        }
        catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error:", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            Console.WriteLine(ex.Message);
            return null;
        }
        return Library;
    }

    public ILibrary GetLibrary() {
        return Library;
    }

    private void _AutoLoad(List<IFileInfo> files) {
        var orderdlist = files.OrderByDescending(x => x.LastModified);
        foreach(var item in orderdlist) {
            if (SetLibrary(item.PhysicalPath) != null) {
                _xmlProvider.SetInProduction(item);
                return;
            }
        }
    }
}