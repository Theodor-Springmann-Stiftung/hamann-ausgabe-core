namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;

public class HaDocumentWrapper : IHaDocumentWrappper {
    private ILibrary Library;
    private IXMLProvider _xmlProvider;

    public int StartYear { get; private set; }
    public int EndYear { get; private set; }

    public HaDocumentWrapper(IXMLProvider xmlProvider, IConfiguration configuration) {
        _xmlProvider = xmlProvider;
        StartYear = configuration.GetValue<int>("AvailableStartYear");
        EndYear = configuration.GetValue<int>("AvailableEndYear");
        var filelist = xmlProvider.GetHamannFiles();
        if (filelist != null && filelist.Any()) {
            _AutoLoad(filelist);
        }

        // Use Fallback library
        if (Library == null) {
            var options = new HaWeb.Settings.HaDocumentOptions();
            if (SetLibrary(options.HamannXMLFilePath) == null) {
                throw new Exception("Die Fallback Hamann.xml unter " + options.HamannXMLFilePath + " kann nicht geparst werden.");
            }
        }
    }

    public ILibrary ResetLibrary() {
        Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { AvailableYearRange = (StartYear, EndYear) });
        return Library;
    }

    public ILibrary? SetLibrary(string filepath, ModelStateDictionary? ModelState = null) {
        try 
        {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath, AvailableYearRange = (StartYear, EndYear) });
        }
        catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Das Dokument konnte nicht geparst werden: " + ex.Message);
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