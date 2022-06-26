using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaDocument.Models;

namespace HaWeb.Controllers;

public class Briefecontroller : Controller {
    [BindProperty(SupportsGet = true)]
    public string? id { get; set; }

    // DI
    private IHaDocumentWrappper _lib;
    private IReaderService _readerService;

    public Briefecontroller(IHaDocumentWrappper lib, IReaderService readerService) {
        _lib = lib;
        _readerService = readerService;
    }

    [Route("Briefe")]
    [Route("Briefe/{id?}")]
    public IActionResult Index(string? id) {
        // Setup settings and variables
        var lib = _lib.GetLibrary();
        var url = "/Briefe/";
        var defaultID = "1";

        // Normalisation and Validation, (some) data aquisition
        if (id == null) return Redirect(url + defaultID);
        id = id.ToLower();
        var preliminarymeta = lib.Metas.Where(x => x.Value.Autopsic == id);
        if (preliminarymeta == null || !preliminarymeta.Any()) return error404();

        // Get all neccessary data
        var index = preliminarymeta.First().Key;
        var meta = preliminarymeta.First().Value;
        var text = lib.Letters.ContainsKey(index) ? lib.Letters[index] : null;
        var marginals = lib.MarginalsByLetter.Contains(index) ? lib.MarginalsByLetter[index] : null;
        var tradition = lib.Traditions.ContainsKey(index) ? lib.Traditions[index] : null;
        var editreasons = lib.Editreasons.ContainsKey(index) ? lib.EditreasonsByLetter[index] : null; // TODO: Order
        var hands = lib.Hands.ContainsKey(index) ? lib.Hands[index] : null;
        var nextmeta = meta != lib.MetasByDate.Last() ? lib.MetasByDate.ItemRef(lib.MetasByDate.IndexOf(meta) + 1) : null;
        var prevmeta = meta != lib.MetasByDate.First() ? lib.MetasByDate.ItemRef(lib.MetasByDate.IndexOf(meta) - 1) : null;

        // More Settings and variables
        ViewData["Title"] = "Brief " + id.ToLower();
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Brief " + id.ToLower();
        ViewData["Filename"] = "HKB_" + meta.Autopsic + ".pdf";

        // Model creation
        var hasMarginals = false;
        if (marginals != null && marginals.Any()) hasMarginals = true;
        var model = new BriefeViewModel(id, index, generateMetaViewModel(lib, meta, hasMarginals));
        if (nextmeta != null) model.MetaData.Next = (generateMetaViewModel(lib, nextmeta, false), url + nextmeta.Autopsic);
        if (prevmeta != null) model.MetaData.Prev = (generateMetaViewModel(lib, prevmeta, false), url + prevmeta.Autopsic);
        if (hands != null && hands.Any()) model.ParsedHands = HaWeb.HTMLHelpers.LetterHelpers.CreateHands(lib, hands);
        if (editreasons != null && editreasons.Any()) model.ParsedEdits = HaWeb.HTMLHelpers.LetterHelpers.CreateEdits(lib, _readerService, editreasons);
        if (tradition != null && !String.IsNullOrWhiteSpace(tradition.Element)) {
            var parsedTraditions = HaWeb.HTMLHelpers.LetterHelpers.CreateTraditions(lib, _readerService, marginals, tradition, hands, editreasons);
            (model.ParsedTradition, model.ParsedMarginals, model.MinWidthTrad) = (parsedTraditions.sb_tradition.ToString(), parsedTraditions.ParsedMarginals, parsedTraditions.minwidth);
        }
        if (text != null && !String.IsNullOrWhiteSpace(text.Element)) {
            var parsedLetter = HaWeb.HTMLHelpers.LetterHelpers.CreateLetter(lib, _readerService, meta, text, marginals, hands, editreasons);
            (model.ParsedText, model.MinWidth) = (parsedLetter.sb_lettertext.ToString(), parsedLetter.minwidth);
            if (model.ParsedMarginals != null && parsedLetter.ParsedMarginals != null) model.ParsedMarginals.AddRange(parsedLetter.ParsedMarginals);
            else model.ParsedMarginals = parsedLetter.ParsedMarginals;
            if (parsedLetter.Startline != "-1" && parsedLetter.Startline != "1" && model.MetaData.ParsedZHString != null)
                model.MetaData.ParsedZHString += " / " + parsedLetter.Startline;
            if (String.IsNullOrWhiteSpace(model.ParsedText))
                model.MetaData.HasText = false;
        }

        // Return
        return View(model);
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }

    private BriefeMetaViewModel generateMetaViewModel(ILibrary lib, Meta meta, bool hasMarginals) {
        var senders = meta.Senders.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var recivers = meta.Receivers.Select(x => lib.Persons[x].Name) ?? new List<string>();
        var zhstring = meta.ZH != null ? HaWeb.HTMLHelpers.LetterHelpers.CreateZHString(meta) : null;
        return new BriefeMetaViewModel(meta, hasMarginals) {
            ParsedZHString = zhstring,
            ParsedSenders = HTMLHelpers.StringHelpers.GetEnumerationString(senders),
            ParsedReceivers = HTMLHelpers.StringHelpers.GetEnumerationString(recivers)
        };
    }
}