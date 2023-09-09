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
using System.Xml.Linq;
using System.Diagnostics;

public class HaDocumentWrapper : IHaDocumentWrappper {
    private IFileInfo _ActiveFile;
    private ILibrary? Library;
    private IXMLInteractionService _xmlService;
    private int _startYear;
    private int _endYear;
    private List<Person>? _availablePersons;
    private Dictionary<string, Person>? _personsWithLetters;

    // public List<SearchHelpers.CollectedItem>? SearchableLetters { get; private set; }

    public HaDocumentWrapper(IXMLInteractionService service, IConfiguration configuration) {
        _xmlService = service;
        ParseConfiguration(configuration);
    }

    public void ParseConfiguration(IConfiguration configuration) {
        _startYear = configuration.GetValue<int>("AvailableStartYear");
        _endYear = configuration.GetValue<int>("AvailableEndYear");
    }

    public List<Person>? GetAvailablePersons() => _availablePersons;

    public Dictionary<string, Person>? GetPersonsWithLetters() => _personsWithLetters;

    public int GetStartYear() => _startYear;

    public int GetEndYear() => _endYear;

    public IFileInfo GetActiveFile() => _ActiveFile;

    public ILibrary? SetLibrary(IFileInfo? file, XDocument? doc, ModelStateDictionary? ModelState = null) {
        // Handle null on file & doc
        var path = file == null ? new HaWeb.Settings.HaDocumentOptions().HamannXMLFilePath : file.PhysicalPath;
        if (doc == null) doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);

        // 1. Parse the Document, create search Index
        if (_xmlService != null) 
            _xmlService.CreateSearchables(doc);
        // 2. Set ILibrary
        try {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = path, AvailableYearRange = (_startYear, _endYear) }, doc.Root);
        } catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            return null;
        }

        // 3a. Set Available Persons
        var persons = Library.Metas.SelectMany(x => x.Value.Senders.Union(x.Value.Receivers)).Distinct();
        _availablePersons = persons.Select(x => Library.Persons[x]).OrderBy(x => x.Surname).ThenBy(x => x.Prename).ToList();

        // 3b. Setup a Dictionary with available Person ovierview Pages
        _personsWithLetters = new Dictionary<string, Person>();
        var availablePersonPages = Library.Persons.Where(x => !String.IsNullOrWhiteSpace(x.Value.Komm));
        foreach (var p in availablePersonPages) {
            if (!_personsWithLetters.ContainsKey(p.Value.Komm!)) {
                _personsWithLetters.Add(p.Value.Komm, p.Value);
            }
        }

        // 4. Set info on loaded file
        _ActiveFile = file;
        return Library;
    }

    public ILibrary? GetLibrary() {
        return Library;
    }
}