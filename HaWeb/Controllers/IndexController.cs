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

public class IndexController : Controller {
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;
    private IXMLInteractionService _xmlService;
    private int _lettersForPage;
    private int _endYear;

    public IndexController(IXMLFileProvider _, IHaDocumentWrappper lib, IReaderService readerService, IXMLInteractionService service, IConfiguration config) {
        _lib = lib;
        _readerService = readerService;
        _xmlService = service;
        _lettersForPage = config.GetValue<int>("LettersOnPage");
        _endYear = config.GetValue<int>("AvailableEndYear");
    }

    [Route("/HKB/{letterno}")]
    public IActionResult GoTo(string letterno) {
        if (String.IsNullOrWhiteSpace(letterno)) return _error404();
        letterno = letterno.Trim();
        var lib = _lib.GetLibrary();
        var letter = lib.Metas.ContainsKey(letterno) ? lib.Metas[letterno] : null;
        if (letter != null)
            return RedirectToAction("Index", "Briefe", new { id = letterno });
        return _error404();
    }

    [Route("/HKB/{zhvolume}/{zhpage}")]
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
                autopsic = lib.Metas[letters.First()].ID;
            }
            if (autopsic == null) return _error404();
            return RedirectToAction("Index", "Briefe", new { id = autopsic });
        }
        if (letters != null && letters.Any()) {
            var metas = lib.Metas.Where(x => letters.Contains(x.Key)).Select(x => x.Value);
            if (metas == null) return _error404();
            var metasbyyear = metas.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
            return _paginateSend(lib, 0, metasbyyear, null, null, zhvolume, zhpage);
        }
        return _error404();
    }

    [Route("/HKB/")]
    // Order of actions:
    // Filter, sort by year, paginate, sort by Meta.Sort & .Order, parse
    public IActionResult Index(int page = 0) {
        var lib = _lib.GetLibrary();
        List<IGrouping<int, Meta>>? metasbyyear = null;
        metasbyyear = lib.MetasByYear.OrderBy(x => x.Key).ToList();
        return _paginateSend(lib, page, metasbyyear);
    }

    [Route("/HKB/Person/{person}")]
    public IActionResult Person(string person, int page = 0) {
        var lib = _lib.GetLibrary();
        if (String.IsNullOrWhiteSpace(person)) return _error404();
        person = person.Trim();
        if (!lib.Persons.ContainsKey(person)) return _error404();
        List<IGrouping<int, Meta>>? metasbyyear = null;
        Person p = lib.Persons[person];
        CommentModel? comment = null;
        if (p.Komm != null) comment = _getPersonComment(p.Komm);
        var letters = lib.Metas
            .Where(x => x.Value.Senders.Contains(person) || x.Value.Receivers.Contains(person))
            .Select(x => x.Value);
        if (letters == null) return _error404();
        metasbyyear = letters.ToLookup(x => x.Sort.Year).OrderBy(x => x.Key).ToList();
        return _paginateSend(lib, page, metasbyyear, person, comment);
    }


    private List<(string Key, string Person)>? _getAvailablePersons() {
        if (_lib.GetAvailablePersons() == null || !_lib.GetAvailablePersons()!.Any()) return null;
        return _lib.GetAvailablePersons()!
            .Select(x => (x.Index, x.Name))
            .ToList();
    }

    internal static List<(int StartYear, int EndYear)>? Paginate(List<IGrouping<int, Meta>>? letters, int lettersForPage) {
        if (letters == null || !letters.Any()) return null;
        List<(int StartYear, int EndYear)>? res = null;
        int startyear = 0;
        int count = 0;
        foreach (var letterlist in letters) {
            if (count == 0) {
                startyear = letterlist.Key;
            }
            count += letterlist.Count();
            if (count >= lettersForPage) {
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
        CommentModel? personcomment = null,
        string? zhvolume = null,
        string? zhpage = null) {
        var pages = Paginate(metasbyyear, _lettersForPage);
        if (pages != null && page >= pages.Count) return _error404();
        if (pages == null && page > 0) return _error404();
        List<(int Year, List<BriefeMetaViewModel> LetterList)>? letters = null;
        if (pages != null)
            letters = metasbyyear
                .Where(x => x.Key >= pages[page].StartYear && x.Key <= pages[page].EndYear)
                .Select(x => (x.Key, x
                    .Select(y => Briefecontroller.GenerateMetaViewModel(lib, y, false))
                    .OrderBy(x => x.Meta.Sort)
                    .ThenBy(x => x.Meta.Order)
                    .ToList()))
                .ToList();
        List<(string Volume, List<string> Pages)>? availablePages = null;
        availablePages = lib.Structure.Where(x => x.Key != "-1").Select(x => (x.Key, x.Value.Select(x => x.Key).ToList())).ToList();
        zhvolume = zhvolume == null ? "1" : zhvolume;

        var lastletter = lib.MetasByDate.Last();
        var model = new IndexViewModel(
            letters, 
            page,
            _endYear.ToString(),
            "ZH " + HTMLHelpers.ConversionHelpers.ToRoman(Int32.Parse(lastletter.ZH.Volume)) + ", S. " + lastletter.ZH.Page,
            pages,
            _getAvailablePersons(), 
            availablePages.OrderBy(x => x.Volume).ToList(), 
            zhvolume, 
            zhpage, 
            person
        );
        model.PersonComment = personcomment;
        return View("~/Views/HKB/Dynamic/Index.cshtml", model);
    }

    private CommentModel? _getPersonComment(string comment) {
        var lib = _lib.GetLibrary();
        if (!lib.Comments.ContainsKey(comment)) return null;
        var comm = lib.Comments[comment];
        var parsedComment = HTMLHelpers.CommentHelpers.CreateHTML(lib, _readerService, comm, "neuzeit", Settings.ParsingState.CommentType.Comment, true); // Maybe true for Backlinks
        List<string>? parsedSubComments = null;
        if (comm.Kommentare != null) {
            parsedSubComments = new List<string>();
            foreach (var subcomm in comm.Kommentare.OrderBy(x => x.Value.Order)) {
                parsedSubComments.Add(HTMLHelpers.CommentHelpers.CreateHTML(lib, _readerService, subcomm.Value, "neuzeit", Settings.ParsingState.CommentType.Subcomment, true));
            }
        }
        return new CommentModel(parsedComment, parsedSubComments, comment);
    }

    private IActionResult _error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }
}
