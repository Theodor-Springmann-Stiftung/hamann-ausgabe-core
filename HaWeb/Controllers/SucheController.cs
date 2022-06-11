using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaDocument.Models;
using System.Collections.Specialized;

namespace HaWeb.Controllers;

public class SucheController : Controller {
    private IHaDocumentWrappper _lib;
    private int _lettersForPage;

    public SucheController(IHaDocumentWrappper lib, IConfiguration config) {
        _lib = lib;
        _lettersForPage = config.GetValue<int>("LettersOnPage");
    }

    [Route("Suche")]
    // Filter, Order By Year, Paginate, Order By Date and by Order, Parse
    public IActionResult Index() {
        var lib = _lib.GetLibrary();
        return View();
    }

    private BriefeMetaViewModel generateMetaViewModel(ILibrary lib, Meta meta) {
        var hasMarginals = lib.MarginalsByLetter.Contains(meta.Index) ? true : false;
        var senders = meta.Senders.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var recivers = meta.Receivers.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var zhstring = meta.ZH != null ? HaWeb.HTMLHelpers.LetterHelpers.CreateZHString(meta) : null;
        return new BriefeMetaViewModel(meta, hasMarginals, false) {
            ParsedZHString = zhstring,
            ParsedSenders = HTMLHelpers.StringHelpers.GetEnumerationString(senders),
            ParsedReceivers = HTMLHelpers.StringHelpers.GetEnumerationString(recivers)
        };
    }
    
    private List<(int StartYear, int EndYear)>? Paginate(ILookup<int, List<Meta>>? letters) {
        if (letters == null || !letters.Any()) return null;
        var orderedl = letters.OrderBy(x => x.Key);
        List<(int StartYear, int EndYear)>? res = null;
        int startyear = 0;
        int count = 0;
        foreach (var letterlist in orderedl) {
            count += letterlist.Count();
            if (count == 0) {
                startyear = letterlist.Key;
            }
            if (count >= _lettersForPage) {
                if (res == null) res = new List<(int StartYear, int EndYear)>();
                res.Add((startyear, letterlist.Key));
                count = 0;
            }
        }
        return res;
    }
}
