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

    [Route("Suche/{letterno}")]
    public IActionResult GoTo(string letterno) {
        if (String.IsNullOrWhiteSpace(letterno)) return _error404();
        letterno = letterno.Trim();
        var lib = _lib.GetLibrary();
        var letter = lib.Metas.Where(x => x.Value.Autopsic == letterno);
        if (letter != null)
            return RedirectToAction("Index", "Briefe", new { id = letterno });
        return _error404();
    }

    [Route("Suche/{zhvolume}/{zhpage}")]
    public IActionResult GoToZH(string zhvolume, string zhpage) {
        if (String.IsNullOrWhiteSpace(zhvolume) || String.IsNullOrWhiteSpace(zhpage)) return _error404();
        zhvolume = zhvolume.Trim();
        zhpage = zhpage.Trim();
        var lib = _lib.GetLibrary();
        var pages = lib.Structure.ContainsKey(zhvolume) ? lib.Structure[zhvolume] : null;
        if (pages == null) return _error404();
        var lines = pages.ContainsKey(zhpage) ? pages[zhpage] : null;
        if (lines == null) return _error404();
        var letters = lines.Aggregate(new HashSet<string>(), (x, y) => { x.Add(y.Value); return x; });
        if (letters != null && letters.Any() && letters.Count == 1) {
            string? autopsic = null;
            if (lib.Metas.ContainsKey(letters.First())) {
                autopsic = lib.Metas[letters.First()].Autopsic;
            }
            if (autopsic == null) return _error404();
            return RedirectToAction("Index", "Briefe", new { id = autopsic });
        }
        if (letters != null && letters.Any()) {
            var metas = lib.Metas.Where(x => letters.Contains(x.Key)).Select(x => x.Value);
            if (metas == null) return _error404();
            var metasbyyear = metas.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
            return _paginateSend(lib, 0, metasbyyear, null, zhvolume, zhpage);
        }
        return _error404();
    }

    [Route("Suche")]
    // Order of actions:
    // Filter, sort by year, paginate, sort by Meta.Sort & .Order, parse
    public IActionResult Index(string? search, int page = 0) {
        var lib = _lib.GetLibrary();
        List<IGrouping<int, Meta>>? metasbyyear = null;
        if (search != null) {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            search = search.Trim();
            var res = _xmlService.SearchCollection("letters", search, _readerService);
            if (res == null || !res.Any()) return _error404();
            var ret = res.ToDictionary(
                x => x.Index,
                x => x.Results
                    .Select(y => new SearchResult(search, x.Index) { Page = y.Page, Line = y.Line, Preview = y.Preview })
                    .ToList()
                );
            var keys = res.Select(x => x.Index).Where(x => lib.Metas.ContainsKey(x)).Select(x => lib.Metas[x]);
            var letters = keys.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
            stopwatch.Stop();
            Console.WriteLine("SEARCH: " + stopwatch.ElapsedMilliseconds);
            return _paginateSend(lib, page, letters, null, null, null, search, ret);
        }
        metasbyyear = lib.MetasByYear.OrderBy(x => x.Key).ToList();
        return _paginateSend(lib, page, metasbyyear);
    }

    [Route("Suche/Person/{person}")]
    public IActionResult Person(string person, int page = 0) {
        if (String.IsNullOrWhiteSpace(person)) return _error404();
        person = person.Trim();
        var lib = _lib.GetLibrary();
        List<IGrouping<int, Meta>>? metasbyyear = null;
        var letters = lib.Metas
            .Where(x => x.Value.Senders.Contains(person) || x.Value.Receivers.Contains(person))
            .Select(x => x.Value);
        if (letters == null) return _error404();
        metasbyyear = letters.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
        return _paginateSend(lib, page, metasbyyear, person);
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

    private List<(int StartYear, int EndYear)>? _paginate(List<IGrouping<int, Meta>>? letters) {
        if (letters == null || !letters.Any()) return null;
        List<(int StartYear, int EndYear)>? res = null;
        int startyear = 0;
        int count = 0;
        foreach (var letterlist in letters) {
            if (count == 0) {
                startyear = letterlist.Key;
            }
            count += letterlist.Count();
            if (count >= _lettersForPage) {
                if (res == null) res = new List<(int StartYear, int EndYear)>();
                res.Add((startyear, letterlist.Key));
                count = 0;
            }
            if (letterlist == letters.Last()) {
                if (res == null) res = new List<(int StartYear, int EndYear)>();
                res.Add((startyear, letterlist.Key));
            }
        }
        return res;
    }

    private IActionResult _paginateSend(
        ILibrary lib,
        int page,
        List<IGrouping<int, Meta>> metasbyyear,
        string? person = null,
        string? zhvolume = null,
        string? zhpage = null,
        string? activeSearch = null,
        Dictionary<string, List<SearchResult>>? searchResults = null) {
        var pages = _paginate(metasbyyear);
        if (pages != null && page >= pages.Count) return _error404();
        if (pages == null && page > 0) return _error404();
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters = null;
        if (pages != null)
            letters = metasbyyear
                .Where(x => x.Key >= pages[page].StartYear && x.Key <= pages[page].EndYear)
                .Select(x => (x.Key, x
                    .Select(y => _generateMetaViewModel(lib, y))
                    .OrderBy(x => x.Meta.Sort)
                    .ThenBy(x => x.Meta.Order)
                    .ToList()))
                .ToList();
        List<(string Volume, List<string> Pages)>? availablePages = null;
        availablePages = lib.Structure.Select(x => (x.Key, x.Value.Select(x => x.Key).ToList())).ToList();
        zhvolume = zhvolume == null ? "1" : zhvolume;
        var model = new SucheViewModel(letters, page, pages, _getAvailablePersons(lib), availablePages.OrderBy(x => x.Volume).ToList(), zhvolume, zhpage, activeSearch, searchResults);
        if (person != null) model.ActivePerson = person;
        return View("Index", model);
    }

    private IActionResult _error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}
