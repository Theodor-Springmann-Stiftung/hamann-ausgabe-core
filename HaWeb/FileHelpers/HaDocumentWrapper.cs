namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
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
    private int _startYear;
    private int _endYear;
    private List<Person>? _availablePersons;
    private Dictionary<string, Person>? _personsWithLetters;

    // public List<SearchHelpers.CollectedItem>? SearchableLetters { get; private set; }

    public HaDocumentWrapper(IXMLProvider xmlProvider, IXMLService service, IConfiguration configuration) {
        _xmlProvider = xmlProvider;
        _xmlService = service;
        _startYear = configuration.GetValue<int>("AvailableStartYear");
        _endYear = configuration.GetValue<int>("AvailableEndYear");
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

    public List<Person>? GetAvailablePersons() => _availablePersons;

    public Dictionary<string, Person>? GetPersonsWithLetters() => _personsWithLetters;

    public int GetStartYear() => _startYear;

    public int GetEndYear() => _endYear;

    public void SetEndYear(int end) {
        this._endYear = end;
        SetLibrary(_filepath);
    }

    public ILibrary? SetLibrary(string filepath, ModelStateDictionary? ModelState = null) {
        // 1. Set ILibrary
        try {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath, AvailableYearRange = (_startYear, _endYear) });
        } catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            return null;
        }

        // 1a. Set Available Persons
        var persons = Library.Metas.SelectMany(x => x.Value.Senders.Union(x.Value.Receivers)).Distinct();
        _availablePersons = persons.Select(x => Library.Persons[x]).OrderBy(x => x.Surname).ThenBy(x => x.Prename).ToList();

        // 1b. Setup a Dictionary with available Person ovierview Pages
        
        _personsWithLetters = new Dictionary<string, Person>();
        var availablePersonPages = Library.Persons.Where(x => !String.IsNullOrWhiteSpace(x.Value.Komm));
        foreach (var p in availablePersonPages) {
            if (!_personsWithLetters.ContainsKey(p.Value.Komm!)) {
                _personsWithLetters.Add(p.Value.Komm, p.Value);
            }
        }

        // 2. Set Library in Production, collect some Objects
        if (_xmlService != null) 
            _xmlService.SetInProduction(System.Xml.Linq.XDocument.Load(filepath, System.Xml.Linq.LoadOptions.PreserveWhitespace));

        // 3. Set Filepath
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