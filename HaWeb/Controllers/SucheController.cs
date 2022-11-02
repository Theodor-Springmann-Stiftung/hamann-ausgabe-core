using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using HaXMLReader.Interfaces;
using System.Collections.Specialized;
using HaWeb.XMLParser;

namespace HaWeb.Controllers;

public class SucheController : Controller {
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;
    private IXMLService _xmlService;
    private int _lettersForPage;

    public SucheController(IHaDocumentWrappper lib, IReaderService readerService, IXMLService service, IConfiguration config) {
        _lib = lib;
        _readerService = readerService;
        _xmlService = service;
        _lettersForPage = config.GetValue<int>("LettersOnPage");
    }
    
    [Route("/HKB/Suche")]
    public IActionResult Index(string search, string category = "letters", int page = 0) {
        if (search == null) return _error404();
        var lib = _lib.GetLibrary();

        if (category == "letters") {
            if (String.IsNullOrWhiteSpace(search)) 
                return _paginateSendLetters(lib, page, search, SearchResultType.InvalidSearchTerm, null, null);
            search = search.Trim();
            var res = _xmlService.SearchCollection("letters", search, _readerService);
            if (res == null || !res.Any()) 
                return _paginateSendLetters(lib, page, search, SearchResultType.NotFound, null, null);
            var ret = res.ToDictionary(
                x => x.Index,
                x => x.Results
                    .Select(y => new SearchResult(search, x.Index) { Page = y.Page, Line = y.Line, Preview = y.Preview })
                    .ToList()
                );
            var keys = res.Select(x => x.Index).Where(x => lib.Metas.ContainsKey(x)).Select(x => lib.Metas[x]);
            var letters = keys.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();

            return _paginateSendLetters(lib, page, search, SearchResultType.Success, ret, letters);
        } else if (category == "register") {
            if (String.IsNullOrWhiteSpace(search)) 
                return _paginateSendRegister(lib, page, search, SearchResultType.InvalidSearchTerm, null);
            search = search.Trim();

            List<(string Index, List<(string Page, string Line, string Preview, string Identifier)> Results)>? res = null;
            if (page == 0)
                res = _xmlService.SearchCollection("register-comments", search, _readerService);
            if (page == 1)
                res = _xmlService.SearchCollection("forschung-comments", search, _readerService);
            if (res == null || !res.Any()) 
                return _paginateSendRegister(lib, page, search, SearchResultType.NotFound, null);
            
            return _paginateSendRegister(lib, page, search, SearchResultType.Success, _createComments("neuzeit", res.Select((x) => (x.Index, x.Results.Select((y) => y.Identifier).ToList())).OrderBy(x => x.Index).ToList()));
        }

        return _error404();
    }

    private List<(string Key, string Person)> _getAvailablePersons(ILibrary lib) {
        return lib.Persons
            .OrderBy(x => x.Value.Surname)
            .ThenBy(x => x.Value.Prename)
            .Select(x => (x.Key, x.Value.Name))
            .ToList();
    }

    private BriefeMetaViewModel _generateMetaViewModel(ILibrary lib, Meta meta) {
        var hasMarginals = lib.MarginalsByLetter.Contains(meta.Index) ? true : false;
        var senders = meta.Senders.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var recivers = meta.Receivers.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var zhstring = meta.ZH != null ? HaWeb.HTMLHelpers.LetterHelpers.CreateZHString(meta) : null;
        return new BriefeMetaViewModel(meta, hasMarginals) {
            ParsedZHString = zhstring,
            ParsedSenders = HTMLHelpers.StringHelpers.GetEnumerationString(senders),
            ParsedReceivers = HTMLHelpers.StringHelpers.GetEnumerationString(recivers)
        };
    }

    private List<string>? _paginate(List<(int StartYear, int EndYear)>? pages) {
        return pages != null ? pages.Select(x => {
            if (x.StartYear != x.EndYear) 
                return x.StartYear + "–" + x.EndYear;
            else
                return x.StartYear.ToString();
        }).ToList() : null;
    }

    private List<string>? _paginate(List<string> comments) {
        return null;
    }

    private IActionResult _paginateSendLetters(
        ILibrary lib,
        int page,
        string activeSearch,
        SearchResultType SRT,
        Dictionary<string, List<SearchResult>>? searchResults,
        List<IGrouping<int, Meta>>? metasbyyear
    ) {
        var pages = IndexController.Paginate(metasbyyear, _lettersForPage);
        if (pages != null && page >= pages.Count) return _error404();
        if (pages == null && page > 0) return _error404();
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters = null;
        if (pages != null && metasbyyear != null)
            letters = metasbyyear
                .Where(x => x.Key >= pages[page].StartYear && x.Key <= pages[page].EndYear)
                .Select(x => (x.Key, x
                    .Select(y => _generateMetaViewModel(lib, y))
                    .OrderBy(x => x.Meta.Sort)
                    .ThenBy(x => x.Meta.Order)
                    .ToList()))
                .ToList();
        var model = new SucheViewModel("letters", SRT, page, _paginate(pages), activeSearch, searchResults, letters, null);
        return View("~/Views/HKB/Dynamic/Suche.cshtml", model);
    }

    private IActionResult _paginateSendRegister(
        ILibrary lib,
        int page,
        string activeSearch,
        SearchResultType SRT,
        List<CommentModel> comments) {
            var model = new SucheViewModel("register", SRT, page, new List<string>() { "Allgmeines Register", "Forschungsbibliographie" }, activeSearch, null, null, comments);
            return View("~/Views/HKB/Dynamic/Suche.cshtml", model);
    }


    private List<CommentModel> _createComments(string category, List<(string, List<string>)>? comments) {
        var lib = _lib.GetLibrary();
        var res = new List<CommentModel>();
        if (comments == null) return res;
        foreach (var comm in comments) {
            var commobj = lib.Comments[comm.Item1];
            var parsedComment = HTMLHelpers.CommentHelpers.CreateHTML(lib, _readerService, commobj, category, Settings.ParsingState.CommentType.Comment);
            List<string>? parsedSubComments = new List<string>();
            var distinctList = comm.Item2.Distinct().ToList();
            foreach (var subcomm in distinctList) {
                if (subcomm != comm.Item1) {
                    var subcommobj = lib.SubCommentsByID[subcomm];
                    parsedSubComments.Add(HTMLHelpers.CommentHelpers.CreateHTML(lib, _readerService, subcommobj, category, Settings.ParsingState.CommentType.Subcomment));
                }
            }
            res.Add(new CommentModel(parsedComment, parsedSubComments));
        }
        return res;
    }

    private IActionResult _error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}
