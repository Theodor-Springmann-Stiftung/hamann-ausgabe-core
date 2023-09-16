using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using HaXMLReader.Interfaces;
using System.Collections.Specialized;
using HaWeb.XMLParser;
using HaWeb.Settings.ParsingState;
using System.Text;
using HaWeb.Settings.ParsingRules;
using System.Linq;//AsParallel, ToList
using System.Collections.Generic;//Dictionary

namespace HaWeb.Controllers;

public class SucheController : Controller {
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;
    private IXMLInteractionService _xmlService;
    private int _lettersForPage;

    public SucheController(IHaDocumentWrappper lib, IReaderService readerService, IXMLInteractionService service, IConfiguration config) {
        _lib = lib;
        _readerService = readerService;
        _xmlService = service;
        _lettersForPage = config.GetValue<int>("LettersOnPage");
    }
    
    // Letter Search
    [Route("/HKB/Suche/Briefe/")]
    public IActionResult Briefe(string search, int page = 0, bool? comments = false) {
        var lib = _lib.GetLibrary();

        // Error checking
        if (search == null) return _error404();
        if (String.IsNullOrWhiteSpace(search)) 
            return View("~/Views/HKB/Dynamic/Suche.cshtml", new SucheViewModel(SearchType.Letters, SearchResultType.InvalidSearchTerm, comments, page, null, search, null, null, null, null));
        search = search.Trim();

        // Letter & comment search and search result creation
        var resletter = _xmlService.SearchCollection("letters", search, _readerService, null);
        List<(CollectedItem, List<(string Page, string Line, string Preview, string Identifier)> Results)>? rescomments = null;
        if (comments == true) 
            rescomments = _xmlService.SearchCollection("marginals", search, _readerService, lib);
        
        // Error checking
        if ((resletter == null || !resletter.Any()) && (rescomments == null || !rescomments.Any())) 
            return View("~/Views/HKB/Dynamic/Suche.cshtml", new SucheViewModel(SearchType.Letters, SearchResultType.NotFound, comments, page, null, search, null, null, null, null));
        
        // Metadata aquisition & sorting
        List<Meta>? metas = new List<Meta>();
        if (resletter != null) 
            metas.AddRange(
                resletter
                    .Select(x => x.Item.ID)
                    .Where(x => lib.Metas.ContainsKey(x))
                    .Select(x => lib.Metas[x])
            );
        if (rescomments != null) 
            metas.AddRange( 
                rescomments
                    .Where(x => lib.Metas.ContainsKey(x.Item1.ID.Split('-').First()))
                    .Select(x => lib.Metas[x.Item1.ID.Split('-').First()])
            );

        // Return
        return _paginateSendLettersComments(lib, page, search, comments, SearchResultType.Success, metas.Distinct().ToList(), resletter, rescomments);
    }

    // Register & Bibliography Search
    [Route("/HKB/Suche/Register/")]
    public IActionResult Register(string search) {
        var lib = _lib.GetLibrary();

        // Error checking
        if (search == null) return _error404();
        if (String.IsNullOrWhiteSpace(search)) 
            return _paginateSendRegister(lib, search, SearchType.Register, SearchResultType.InvalidSearchTerm, null);
        search = search.Trim();

        // Search
        var res = _xmlService.SearchCollection("register-comments", search, _readerService, lib);
        if (res == null || !res.Any()) 
            return _paginateSendRegister(lib, search, SearchType.Register, SearchResultType.NotFound, null);

        // Return 
        return _paginateSendRegister(lib, search, SearchType.Register, SearchResultType.Success, _createComments("neuzeit", res.Select((x) => (x.Item.ID, x.Results.Select((y) => y.Identifier).ToList())).OrderBy(x => x.ID).ToList()));

    }

    [Route("/HKB/Suche/Forschung/")]
    public IActionResult Science(string search) {
        var lib = _lib.GetLibrary();

        // Error checking
        if (search == null) return _error404();
        if (String.IsNullOrWhiteSpace(search)) 
            return _paginateSendRegister(lib, search, SearchType.Science, SearchResultType.InvalidSearchTerm, null);
        search = search.Trim();

        // Search
        var res = _xmlService.SearchCollection("forschung-comments", search, _readerService, lib);
        if (res == null || !res.Any()) 
            return _paginateSendRegister(lib, search, SearchType.Science, SearchResultType.NotFound, null);

        // Return 
        return _paginateSendRegister(lib, search, SearchType.Science, SearchResultType.Success, _createComments("neuzeit", res.Select((x) => (x.Item.ID, x.Results.Select((y) => y.Identifier).ToList())).OrderBy(x => x.ID).ToList()));

    }

    private IActionResult _paginateSendLettersComments(
        ILibrary lib,
        int page,
        string search,
        bool? comments,
        SearchResultType SRT,
        List<Meta>? metas,
        List<(CollectedItem, List<(string Page, string Line, string Preview, string Identifier)> Results)>? resletters,
        List<(CollectedItem, List<(string Page, string Line, string Preview, string Identifier)> Results)>? rescomments
    ) {
        // Sorting, get Pages & Error Checking
        var metasbyyear = metas!.Distinct().ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
        var pages = IndexController.Paginate(metasbyyear, _lettersForPage);
        if (pages != null && page >= pages.Count) return _error404();
        if (pages == null && page > 0) return _error404();

        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters = null;
        // Select & Parse Metadata for Letters to be shown on the selected Page
        if (pages != null && metasbyyear != null) 
            letters = metasbyyear
                .Where(x => x.Key >= pages[page].StartYear && x.Key <= pages[page].EndYear)
                .Select(x => (x.Key, x
                    .Select(y => Briefecontroller.GenerateMetaViewModel(lib, y, false))
                    .OrderBy(x => x.Meta.Sort)
                    .ThenBy(x => x.Meta.Order)
                    .ToList()))
                .ToList();
        
        // Generate Search results & previews
        Dictionary<string, List<SearchResult>>? searchResults = new Dictionary<string, List<SearchResult>>();
        Dictionary<string, List<(Marginal, string)>>? parsedMarginals = null;
        if (resletters != null)
            foreach (var res in resletters) {
                if (!searchResults.ContainsKey(res.Item1.ID))
                    searchResults.Add(res.Item1.ID, new List<SearchResult>());
                foreach (var r in res.Results) {
                    if(!searchResults[res.Item1.ID].Where(x => x.Page == r.Page && x.Line == r.Line).Any())
                        searchResults[res.Item1.ID].Add(new SearchResult(search, res.Item1.ID) { Page = r.Page, Line = r.Line, Preview = r.Preview });
                }
                if (searchResults[res.Item1.ID].Any()) {
                    searchResults[res.Item1.ID] = searchResults[res.Item1.ID].OrderBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.Page)).ThenBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.Line)).ToList();
                }
            }
        if (rescomments != null) {
            var marginals = rescomments.Select(x => Marginal.FromXElement(x.Item1.Element)).ToLookup(x => x.Letter);
            var shownletters = letters!.SelectMany(x => x.LetterList.Select(y => y.Meta.ID)).ToHashSet();
            var shownmarginals = marginals!.Where(x => shownletters.Contains(x.Key)).Select(x => (x.Key, x.ToList())).ToList();
            var previews = _xmlService != null ? _xmlService.GetPreviews(shownmarginals, _readerService ,lib) : null;            
            if (previews != null)
                foreach (var p in previews) {
                    if (!searchResults.ContainsKey(p.Item.ID))
                        searchResults.Add(p.Item.ID, new List<SearchResult>());
                    foreach (var res in p.Results) {
                        if (!searchResults[p.Item.ID].Where(x => x.Page == res.Page && x.Line == res.Line).Any())
                            searchResults[p.Item.ID].Add(new SearchResult(search, p.Item.ID) { Page = res.Page, Line = res.Line, Preview = res.Preview });
                    }
                    if (searchResults[p.Item.ID].Any()) {
                        searchResults[p.Item.ID] = searchResults[p.Item.ID].OrderBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.Page)).ThenBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.Line)).ToList();
                    }
                }

            // Parse Marginals
            foreach (var l in marginals) {
                if (parsedMarginals == null && l.Any()) 
                    parsedMarginals = new Dictionary<string, List<(Marginal, string)>>();
                if (l.Any()) {
                    var list = new List<(Marginal, string)>();
                    foreach (var c in l) {
                        var sb = new StringBuilder();
                        var rd = _readerService.RequestStringReader(c.Element);
                        var st = new TextState(lib, _readerService, lib.Metas[c.Letter], null, null, null);
                        new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TextState>(st, rd, sb, TextRules.OTagRules, null, TextRules.CTagRules, TextRules.TRules, TextRules.WhitespaceRules);
                        new HaWeb.HTMLHelpers.LinkHelper(st.Lib, rd, sb, false);
                        rd.Read(); 
                        list.Add((c, sb.ToString()));
                    }
                    parsedMarginals!.Add(l.Key, list);
                }
            }
        }

        // Model Init & Return
        var model = new SucheViewModel(SearchType.Letters, SearchResultType.Success, comments, page, _paginate(pages), search, searchResults, letters, null, parsedMarginals);
        return View("~/Views/HKB/Dynamic/Suche.cshtml", model);
    }

    private IActionResult _paginateSendRegister(
        ILibrary lib,
        string activeSearch,
        SearchType ST,
        SearchResultType SRT,
        List<CommentModel>? comments) {
            // Model init & return
            var model = new SucheViewModel(ST, SRT, null, 0, null, activeSearch, null, null, comments, null);
            return View("~/Views/HKB/Dynamic/Suche.cshtml", model);
    }

    private List<string>? _paginate(List<(int StartYear, int EndYear)>? pages) {
        return pages != null ? pages.Select(x => {
            if (x.StartYear != x.EndYear) 
                return x.StartYear + "–" + x.EndYear;
            else
                return x.StartYear.ToString();
        }).ToList() : null;
    }

    // Select and parse comments to be shown on a page
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
            res.Add(new CommentModel(parsedComment, parsedSubComments, comm.Item1));
        }
        return res;
    }

    private IActionResult _error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}
