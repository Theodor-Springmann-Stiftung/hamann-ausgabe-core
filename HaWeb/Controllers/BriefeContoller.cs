using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaDocument.Models;

namespace HaWeb.Controllers;

public class Briefecontroller : Controller
{
    [BindProperty(SupportsGet = true)]
    public string? id { get; set; }

    // DI
    private ILibrary _lib;
    private IReaderService _readerService;

    public Briefecontroller(ILibrary lib, IReaderService readerService)
    {
        _lib = lib;
        _readerService = readerService;
    }

    [Route("Briefe")]
    [Route("Briefe/{id?}")]
    public IActionResult Index(string? id)
    {
        // Setup settings and variables
        var url = "/Briefe/";
        var defaultID = "1";

        // Normalisation and Validation, (some) data aquisition
        if (id == null) return Redirect(url + defaultID);
        this.id = id.ToLower();
        var preliminarymeta = _lib.Metas.Where(x => x.Value.Autopsic == this.id);
        if (preliminarymeta == null || !preliminarymeta.Any()) return error404();

        // Get all neccessary data
        var index = preliminarymeta.First().Key;
        var meta = preliminarymeta.First().Value;
        var text = _lib.Letters.ContainsKey(index) ? _lib.Letters[index] : null;
        var marginals = _lib.MarginalsByLetter.Contains(index) ? _lib.MarginalsByLetter[index] : null;
        var tradition = _lib.Traditions.ContainsKey(index) ? _lib.Traditions[index] : null;
        var editreasons = _lib.Editreasons.ContainsKey(index) ? _lib.EditreasonsByLetter[index] : null; // TODO: Order
        var hands = _lib.Hands.ContainsKey(index) ? _lib.Hands[index] : null;
        var nextmeta = meta != _lib.MetasByDate.Last() ? _lib.MetasByDate.ItemRef(_lib.MetasByDate.IndexOf(meta) + 1) : null;
        var prevmeta = meta != _lib.MetasByDate.First() ? _lib.MetasByDate.ItemRef(_lib.MetasByDate.IndexOf(meta) - 1) : null;

        // More Settings and variables
        ViewData["Title"] = "Brief " + id.ToLower();
        ViewData["SEODescription"] = "Johann Georg Hamann: Kommentierte Briefausgabe. Brief " + id.ToLower();
        ViewData["Filename"] = "HKB_" + meta.Autopsic + ".pdf";

        // Model creation
        var hasMarginals = false;
        if (marginals != null && marginals.Any()) hasMarginals = true;
        var model = new BriefeViewModel(this.id, index, generateMetaViewModel(meta, hasMarginals));
        if (nextmeta != null) model.MetaData.Next = (generateMetaViewModel(nextmeta, false), url + nextmeta.Autopsic);
        if (prevmeta != null) model.MetaData.Prev = (generateMetaViewModel(prevmeta, false), url + prevmeta.Autopsic);
        if (hands != null && hands.Any()) model.ParsedHands = HaWeb.HTMLHelpers.LetterHelpers.CreateHands(_lib, hands);
        if (editreasons != null && editreasons.Any()) model.ParsedEdits = HaWeb.HTMLHelpers.LetterHelpers.CreateEdits(_lib, _readerService, editreasons);
        if (tradition != null && !String.IsNullOrWhiteSpace(tradition.Element)) model.ParsedTradition = HaWeb.HTMLHelpers.LetterHelpers.CreateTraditions(_lib, _readerService, marginals, tradition);
        if (text != null && !String.IsNullOrWhiteSpace(text.Element)) model.ParsedText = HaWeb.HTMLHelpers.LetterHelpers.CreateLetter(_lib, _readerService, meta, text, marginals);

        // Return
        return View(model);
    }

    private IActionResult error404()
    {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }

    private BriefeMetaViewModel generateMetaViewModel(Meta meta, bool hasMarginals)
    {
        var senders = meta.Senders.Select(x => _lib.Persons[x].Name) ?? new List<string>();
        var recivers = meta.Receivers.Select(x => _lib.Persons[x].Name) ?? new List<string>();
        var zhstring = meta.ZH != null ? HaWeb.HTMLHelpers.LetterHelpers.CreateZHString(meta) : null;
        return new BriefeMetaViewModel(meta, hasMarginals)
        {
            ParsedZHString = zhstring,
            ParsedSenders = HTMLHelpers.StringHelpers.GetEnumerationString(senders),
            ParsedReceivers = HTMLHelpers.StringHelpers.GetEnumerationString(recivers)
        };
    }
}