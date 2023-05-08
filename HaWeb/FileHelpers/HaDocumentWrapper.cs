namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using HaXMLReader.Interfaces;
using HaWeb.SearchHelpers;
using HaWeb.XMLParser;
using System.Text;

public class HaDocumentWrapper : IHaDocumentWrappper {
    private ILibrary Library;
    private IXMLProvider _xmlProvider;
    private IXMLService _xmlService;

    private string _filepath;

    public int StartYear { get; private set; }
    public int EndYear { get; private set; }

    // public List<SearchHelpers.CollectedItem>? SearchableLetters { get; private set; }

    public HaDocumentWrapper(IXMLProvider xmlProvider, IXMLService service, IConfiguration configuration) {
        _xmlProvider = xmlProvider;
        _xmlService = service;
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

    public int GetStartYear() => StartYear;

    public int GetEndYear() => EndYear;

    public void SetStartEndYear(int start, int end) {
        this.StartYear = start;
        this.EndYear = end;
        SetLibrary(_filepath);
    }

    public ILibrary ResetLibrary() {
        var options = new HaWeb.Settings.HaDocumentOptions() { AvailableYearRange = (StartYear, EndYear )};
        Library = HaDocument.Document.Create(options);
        _filepath = options.HamannXMLFilePath;
        return Library;
    }

    public ILibrary? SetLibrary(string filepath, ModelStateDictionary? ModelState = null) {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        try {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath, AvailableYearRange = (StartYear, EndYear) });
        } catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            return null;
        }
        sw.Stop();
        Console.WriteLine("ILIB: " + sw.ElapsedMilliseconds);
        sw.Restart();

        if (_xmlService != null) 
            _xmlService.SetInProduction(System.Xml.Linq.XDocument.Load(filepath, System.Xml.Linq.LoadOptions.PreserveWhitespace));
        sw.Stop();
        Console.WriteLine("COLLECTIONS: " + sw.ElapsedMilliseconds);
        _filepath = filepath;
        return Library;
    }

    public ILibrary GetLibrary() {
        return Library;
    }

    private void _AutoLoad(List<IFileInfo> files) {
        var orderdlist = files.OrderByDescending(x => x.LastModified);
        foreach (var item in orderdlist) {
            if (SetLibrary(item.PhysicalPath) != null) {
                _xmlProvider.SetInProduction(item);
                return;
            }
        }
    }

    private string _prepareSearch(HaDocument.Interfaces.ISearchable objecttoseach) {
        return SearchHelpers.StringHelpers.NormalizeWhiteSpace(objecttoseach.Element, ' ', false);
    }
}