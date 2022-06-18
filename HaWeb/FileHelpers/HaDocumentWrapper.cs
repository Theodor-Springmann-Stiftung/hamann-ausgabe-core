namespace HaWeb.FileHelpers;
using HaDocument.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using HaXMLReader.Interfaces;
using HaWeb.SearchHelpers;
using System.Text;

public class HaDocumentWrapper : IHaDocumentWrappper {
    private ILibrary Library;
    private IXMLProvider _xmlProvider;

    public int StartYear { get; private set; }
    public int EndYear { get; private set; }

    public List<SearchHelpers.SeachableItem>? SearchableLetters { get; private set; }

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
        try {
            Library = HaDocument.Document.Create(new HaWeb.Settings.HaDocumentOptions() { HamannXMLFilePath = filepath, AvailableYearRange = (StartYear, EndYear) });
        } catch (Exception ex) {
            if (ModelState != null) ModelState.AddModelError("Error", "Das Dokument konnte nicht geparst werden: " + ex.Message);
            return null;
        }

        var searchableletters = new ConcurrentBag<SearchHelpers.SeachableItem>();
        var letters = Library.Letters.Values;

        Parallel.ForEach(letters, letter => {
            var o = new SearchHelpers.SeachableItem(letter.Index, _prepareSearch(letter));
            searchableletters.Add(o);
        });

        this.SearchableLetters = searchableletters.ToList();

        return Library;
    }

    public List<(string Index, List<(string Page, string Line, string Preview)> Results)>? SearchLetters(string searchword, IReaderService reader) {
        if (SearchableLetters == null) return null;
        var res = new ConcurrentBag<(string Index, List<(string Page, string Line, string preview)> Results)>();
        var sw = StringHelpers.NormalizeWhiteSpace(searchword.Trim());
        Parallel.ForEach(SearchableLetters, (letter) => {
            var state = new SearchState(sw);
            var rd = reader.RequestStringReader(letter.SearchText);
            var parser = new HaWeb.HTMLParser.LineXMLHelper<SearchState>(state, rd, new StringBuilder(), null, null, null, SearchRules.TextRules, SearchRules.WhitespaceRules);
            rd.Read();
            if (state.Results != null)
                res.Add((
                    letter.Index,
                    state.Results.Select(x => (
                        x.Page,
                        x.Line,
                        parser.Lines != null ?
                            parser.Lines
                            .Where(y => y.Page == x.Page && y.Line == x.Line)
                            .Select(x => x.Text)
                            .FirstOrDefault(string.Empty)
                            : ""
                    )).ToList()));
        });
        return res.ToList();
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