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
    }

    public IActionResult Register(string? id)
    {
        // Setup settings and variables
        var url = "/Register/Register/";
        var category = "neuzeit";
        var defaultLetter = "A";
        var title = "Allgemeines Register";
        ViewData["Title"] = "Allgemeines Register";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Personen-, Sach- und Ortsregister.";

        // Normalisation and validation
        if (id == null) return Redirect(url + defaultLetter);
        normalizeID(id, defaultLetter);
        if (!_lib.CommentsByCategoryLetter[category].Contains(this.id)) return error404();

        // Data aquisition and validation
        var comments = _lib.CommentsByCategoryLetter[category][this.id].OrderBy(x => x.Index);
        var availableCategories = _lib.CommentsByCategoryLetter[category].Select(x => (x.Key.ToUpper(), url + x.Key.ToUpper())).OrderBy(x => x.Item1).ToList();
        if (comments == null) return error404();

        // Parsing
        var res = new List<CommentModel>();
        foreach (var comm in comments)
        {
            var parsedComment = HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, comm);
            List<string>? parsedSubComments = null;
            if (comm.Kommentare != null)
            {
                parsedSubComments = new List<string>();
                foreach (var subcomm in comm.Kommentare.OrderBy(x => x.Value.Order))
                {
                    parsedSubComments.Add(HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, subcomm.Value));
                }
            }
            res.Add(new CommentModel(parsedComment, parsedSubComments));
        }

        // Model instantiation
        var model = new RegisterViewModel(category, this.id, res, title)
        {
            AvailableCategories = availableCategories,
        };

        // Return
        return View("Index", model);
    }

    public IActionResult Bibelstellen(string? id)
    {
        // Setup settings and variables
        var url = "/Register/Bibelstellen/";
        var category = "bibel";
        var defaultLetter = "AT";
        var title = "Bibelstellenregister";
        ViewData["Title"] = "Bibelstellenregister";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Bibelstellenregister.";

        // Normalisation and Validation
        if (id == null) return Redirect(url + defaultLetter);
        normalizeID(id, defaultLetter);
        if (this.id != "AT" && this.id != "AP" && this.id != "NT") return error404();
        
        // Data aquisition and validation
        var comments = _lib.CommentsByCategory[category].ToLookup(x => x.Index.Substring(0, 2).ToUpper())[this.id].OrderBy(x => x.Order);
        var availableCategories = new List<(string, string)>() { ("Altes Testament", url + "AT"), ("Apogryphen", url + "AP"), ("Neues Testament", url + "NT") };
        if (comments == null) return error404();

        // Parsing
        var res = new List<CommentModel>();
        foreach (var comm in comments)
        {
            var parsedComment = HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, comm);
            List<string>? parsedSubComments = null;
            if (comm.Kommentare != null)
            {
                parsedSubComments = new List<string>();
                foreach (var subcomm in comm.Kommentare.OrderBy(x => x.Value.Lemma.Length).ThenBy(x => x.Value.Lemma))
                {
                    parsedSubComments.Add(HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, subcomm.Value));
                }
            }
            res.Add(new CommentModel(parsedComment, parsedSubComments));
        }

        // Model instantiation
        var model = new RegisterViewModel(category, this.id, res, title)
        {
            AvailableCategories = availableCategories,
        };

        // Return
        return View("Index", model);
    }

    public IActionResult Forschung(string? id)
    {
        // Setup settings and variables
        var url = "/Register/Forschung/";
        var category = "forschung";
        var defaultLetter = "A";
        var title = "Forschungsbibliographie";
        ViewData["Title"] = "Forschungsbibliographie";
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Forschungsbibliographie.";

        // Normalisation and Validation
        if (id == null) return Redirect(url + defaultLetter);
        normalizeID(id, defaultLetter);
        if (this.id != "EDITIONEN" && !_lib.CommentsByCategoryLetter[category].Contains(this.id)) return error404();
        if (this.id == "EDITIONEN" && !_lib.CommentsByCategoryLetter.Keys.Contains(this.id.ToLower())) return error404();
        
        // Data aquisition and validation
        IOrderedEnumerable<Comment>? comments = null;
        if (this.id == "EDITIONEN") {
            comments = _lib.CommentsByCategory[this.id.ToLower()].OrderBy(x => x.Index);
        } 
        else {
            comments = _lib.CommentsByCategoryLetter[category][this.id].OrderBy(x => x.Index);
        }
        var availableCategories = _lib.CommentsByCategoryLetter[category].Select(x => (x.Key.ToUpper(), url + x.Key.ToUpper())).OrderBy(x => x.Item1).ToList();
        var AvailableSideCategories = new List<(string, string)>() { ("Editionen", "Editionen") };
        if (comments == null) return error404();

        // Parsing
        var res = new List<CommentModel>();
        foreach (var comm in comments)
        {
            var parsedComment = HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, comm);
            List<string>? parsedSubComments = null;
            if (comm.Kommentare != null)
            {
                parsedSubComments = new List<string>();
                foreach (var subcomm in comm.Kommentare.OrderBy(x => x.Value.Order))
                {
                    parsedSubComments.Add(HTMLHelpers.CommentHelpers.CreateHTML(_lib, _readerService, subcomm.Value));
                }
            }
            res.Add(new CommentModel(parsedComment, parsedSubComments));
        }

        // Model instantiation
        var model = new RegisterViewModel(category, this.id, res, title)
        {
            AvailableCategories = availableCategories,
            AvailableSideCategories = AvailableSideCategories
        };

        // Return
        return View("Index", model);
    }

    private void normalizeID(string? id, string defaultid)
    {
        this.id = this.id.ToUpper();
    }

    private bool validationCheck(HashSet<string> permitted)
    {
        if (!permitted.Contains(id))
        {
            return false;
        }
        return true;
    }

    private IActionResult error404()
    {
        Response.StatusCode = 404;
        return Redirect("/Error404");
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