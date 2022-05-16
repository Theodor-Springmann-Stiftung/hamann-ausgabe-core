using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using System.Text;
using HaXMLReader;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Collections.Concurrent;

namespace HaWeb.Controllers;

[Route("Register/[action]/{id?}")]
public class RegisterController : Controller
{
    private static HashSet<string> _permittedRegister;
    private static HashSet<string> _permittedBibelstellen;
    private static HashSet<string> _permittedForschung;

    [BindProperty(SupportsGet = true)]
    public string? search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? id { get; set; }

    // DI
    private ILibrary _lib;
    private IReaderService _readerService;

    public RegisterController(ILibrary lib, IReaderService readerService)
    {
        _lib = lib;
        _readerService = readerService;

        _permittedRegister = new HashSet<string>();
        _lib.CommentsByCategoryLetter["neuzeit"].Select(x => _permittedRegister.Add(x.Key));

        _permittedForschung = new HashSet<string>();
        _lib.CommentsByCategoryLetter["forschung"].Select(x => _permittedForschung.Add(x.Key));
        _permittedForschung.Add("Editionen");

        _permittedBibelstellen = new HashSet<string>();
        _permittedBibelstellen.Add("NT");
        _permittedBibelstellen.Add("AP");
        _permittedBibelstellen.Add("AT");
    }

    public IActionResult Register(string? id)
    {
        var category = "neuzeit";
        var defaultLetter = "A";
        normalizeID(id, defaultLetter);
        ViewData["Title"] = "Allgemeines Register";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Personen-, Sach- und Ortsregister.";
        return standardModel(category, id, defaultLetter, new RegisterViewModel(), _permittedRegister);
    }

    public IActionResult Bibelstellen(string? id)
    {
        var category = "bibel";
        var defaultLetter = "AT";
        normalizeID(id, defaultLetter);
        ViewData["Title"] = "Bibelstellenregister";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Bibelstellenregister.";
        var model = new RegisterViewModel() {
            AvailableCategories = new List<(string, string)>() { ("Altes Testament", "AT"), ("Apogryphen", "AP"), ("Neues Testament", "NT") },
            Comments = _lib.CommentsByCategory["bibel"].ToLookup(x => x.Index.Substring(0, 2).ToUpper()).Contains(id) ?
                    _lib.CommentsByCategory["bibel"].ToLookup(x => x.Index.Substring(0, 2).ToUpper())[id].Select(x => new CommentModel(x)).OrderBy(x => x.Comment.Order).ToList() : null,
        };
        return standardModel(category, id, defaultLetter, new RegisterViewModel(), _permittedBibelstellen);
    }

    public IActionResult Forschung(string? id)
    {
        var category = "forschung";
        var defaultLetter = "A";
        normalizeID(id, defaultLetter);
        if (id != null && id.ToUpper() == "EDITIONEN") category = "editionen";
        var model = new RegisterViewModel()
        {
            AvailableSideCategories = new List<(string, string)>() { ("Editionen", "Editionen") },
        };
        ViewData["Title"] = "Forschungsbibliographie";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Forschungsbibliographie.";
        return standardModel(category, id, defaultLetter, new RegisterViewModel(), _permittedForschung);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private IActionResult standardModel(string category, string? id, string defaultid, HaWeb.Models.RegisterViewModel model, HashSet<string> permitted)
    {
        if (!validationCheck(permitted)) {
            Response.StatusCode = 404;
            return Redirect("/Error404");
        }
        model.Category = category;
        model.Id = id;
        model.Search = search ?? "";
        model.Comments = model.Comments ?? _lib.CommentsByCategoryLetter[category][id].Select(x => new CommentModel(x)).OrderBy(x => x.Comment.Index).ToList();
        model.AvailableCategories = model.AvailableCategories ?? _lib.CommentsByCategoryLetter[category].Select(x => (x.Key.ToUpper(), x.Key.ToUpper())).ToList();
        model.AvailableCategories.Sort();

        foreach (var k in model.Comments)
            k.SetHTML(_lib, _readerService);

        return View("Index", model);
    }
    
    private void normalizeID(string? id, string defaultid) {
        id = id ?? defaultid;
        id = id.ToUpper();
    }

    private bool validationCheck(HashSet<string> permitted)
    {
        if (!permitted.Contains(id))
        {
            return false;
        }
        return true;
    }

    // private IEnumerable<Comment> Search(IEnumerable<Comment> all) {
    //     var ret = new ConcurrentBag<Comment>();
    //     var cnt = 0;
    //     Parallel.ForEach (all, (comm, state) => {
    //         if (cnt > 150) {
    //             maxSearch = true;
    //             state.Break();
    //         }
    //         if (SearchInComm(comm)) {
    //             ret.Add(comm);
    //             cnt++;
    //         }
    //     });
    //     return ret;
    // }

    // private bool SearchInComm(Comment comment) {
    //     var found = false;
    //     var x = new RegisterSearch(comment, _readerService.RequestStringReader(comment.Entry), search);
    //     found = x.Act();
    //     if (!found) {
    //         x = new RegisterSearch(comment, _readerService.RequestStringReader(comment.Lemma), search);
    //         found = x.Act();
    //     }
    //     if (comment.Kommentare != null)
    //         foreach (var sub in comment.Kommentare) {
    //             if (!found) {
    //                 found = SearchInComm(sub.Value);
    //             }
    //             else break;
    //         }
    //     return found;
    // }
}