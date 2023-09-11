using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HaWeb.Models;
using HaWeb.FileHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using System.Xml.Linq;

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

    [Route("/HKB/Briefe")]
    [Route("/HKB/Briefe/{id?}")]
    public IActionResult Index(string? id, string? search = null) {
        // Setup settings and variables
        var lib = _lib.GetLibrary();
        var url = "/HKB/Briefe/";
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
        ViewData["Title"] = "HKB – Brief " + id.ToLower();
        ViewData["SEODescription"] = "HKB – Brief " + id.ToLower();
        ViewData["Filename"] = "HKB_" + meta.Autopsic + ".pdf";
        if (!string.IsNullOrWhiteSpace(search)) {
            ViewData["Mark"] = search;
        }

        // Model creation
        var hasMarginals = false;
        if (marginals != null && marginals.Any()) hasMarginals = true;
        var model = new BriefeViewModel(id, index, GenerateMetaViewModel(lib, meta, true));
        if (nextmeta != null) model.MetaData.Next = (GenerateMetaViewModel(lib, nextmeta, false), url + nextmeta.Autopsic);
        if (prevmeta != null) model.MetaData.Prev = (GenerateMetaViewModel(lib, prevmeta, false), url + prevmeta.Autopsic);
        if (hands != null && hands.Any()) model.ParsedHands = HaWeb.HTMLHelpers.LetterHelpers.CreateHands(lib, hands);
        if (editreasons != null && editreasons.Any()) model.ParsedEdits = HaWeb.HTMLHelpers.LetterHelpers.CreateEdits(lib, _readerService, editreasons);
        model.DefaultCategory = lib.Apps.ContainsKey("-1") ? lib.Apps["-1"].Category : null;

        List<(string Category, List<Text>)>? texts = null;
        if (text != null && !String.IsNullOrWhiteSpace(text.Element)) {
            var state = HaWeb.HTMLHelpers.LetterHelpers.ParseText(lib, _readerService, text.Element, meta, marginals, hands, editreasons);
            // TODO: it is still hardcoded that <letterText> means <app id="0">
            var textid = "0";
            var category = lib.Apps[textid].Category;
            var name = lib.Apps[textid].Name;
            var t = new Text(id, textid, category, state.minwidth);
            t.ParsedMarginals = state.ParsedMarginals;
            t.ParsedText = state.sb.ToString();
            t.Title = name;
            if (!String.IsNullOrWhiteSpace(t.ParsedText)) {
                if (texts == null) texts = new List<(string, List<Text>)>();
                if(!texts.Where(x => x.Category == category).Any())
                    texts.Add((category, new List<Text>() { t }));
                else
                    texts.Where(x => x.Category == category).First().Item2.Add(t);
            } else {
                model.MetaData.HasText = false;
            }
        } else {
            model.MetaData.HasText = false;
        }
        
        if (tradition != null && !String.IsNullOrWhiteSpace(tradition.Element)) {
            var additions = XElement.Parse(tradition.Element, LoadOptions.PreserveWhitespace).Descendants("app");
            foreach (var a in additions) {
                var app = a.HasAttributes && a.Attribute("ref") != null && lib.Apps.ContainsKey(a.Attribute("ref").Value) ? 
                    lib.Apps[a.Attribute("ref").Value]  : 
                    null;
                if (app != null && !a.IsEmpty) {
                    var state = HaWeb.HTMLHelpers.LetterHelpers.ParseText(lib, _readerService, a, meta, marginals, hands, editreasons);
                    var t = new Text(id, app.Index, app.Category, state.minwidth);
                    t.Title = app.Name;
                    t.ParsedMarginals = state.ParsedMarginals;
                    t.ParsedText = state.sb.ToString();
                    if (texts == null) texts = new List<(string, List<Text>)>();
                    if(!texts.Where(x => x.Category == app.Category).Any())
                        texts.Add((app.Category, new List<Text>() { t }));
                    else
                        texts.Where(x => x.Category == app.Category).First().Item2.Add(t);
                }
            }
        }
        
        model.Texts = texts;

        if (System.IO.File.Exists("./wwwroot/pdf/HKB_" + id + ".pdf")) {
            model.PDFFilePath = "/pdf/HKB_" + id + ".pdf";
        }

        if (System.IO.File.Exists("./wwwroot/pdf/HKB_" + model.MetaData.Meta.Sort.Year + ".pdf")) {
            model.YearPDFFilePath = "/pdf/HKB_" + model.MetaData.Meta.Sort.Year + ".pdf";
        }

        // Return
        return View("~/Views/HKB/Dynamic/Briefe.cshtml", model);
    }

    private IActionResult error404() {
        Response.StatusCode = 404;
        return Redirect("/Error404");
    }

    internal static BriefeMetaViewModel GenerateMetaViewModel(ILibrary lib, Meta meta, bool generatePersonLinks) {
        var hasText = lib.Letters.ContainsKey(meta.Index) ? true : false;
        var hasMarginals = lib.MarginalsByLetter.Contains(meta.Index) ? true : false;
        var senders = meta.Senders.Select(x => lib.Persons[x]).ToList() ?? new List<Person>();
        var receivers = meta.Receivers.Select(x => lib.Persons[x]).ToList() ?? new List<Person>();
        var zhstring = meta.ZH != null ? HaWeb.HTMLHelpers.LetterHelpers.CreateZHString(meta) : null;
        return new BriefeMetaViewModel(meta, hasMarginals) {
            ParsedZHString = zhstring,
            SenderReceiver = generateSendersRecievers(senders, receivers, generatePersonLinks),
            HasText = hasText,
        };
    }


    private static List<(string Sender, string Receiver)>? generateSendersRecievers(List<Person>? senders, List<Person>? receivers, bool generatePersonLinks) {
        var res = new List<(string Sender, string Receiver)>();
        if (senders == null || receivers == null) return null;
        if (!generatePersonLinks) {
            if (senders.Any(x => receivers.Contains(x))) {
                for (var i = 0; i < receivers.Count || i < senders.Count; i++) {
                    res.Add((
                        senders[i].Name,
                        receivers[i].Name
                    ));
                }
            }
            else {
                res.Add((
                    HTMLHelpers.StringHelpers.GetEnumerationString(senders.Select(x => x.Name)),
                    HTMLHelpers.StringHelpers.GetEnumerationString(receivers.Select(x => x.Name))
                ));
            }
        } else {
            if (senders.Any(x => receivers.Contains(x))) {
                for (var i = 0; i < receivers.Count || i < senders.Count; i++) {
                    res.Add((
                        HTMLHelpers.TagHelpers.CreateElement("a", "", "/HKB/Person/" + senders[i].Index) + senders[i].Name + HTMLHelpers.TagHelpers.CreateEndElement("a"),
                        HTMLHelpers.TagHelpers.CreateElement("a", "", "/HKB/Person/" + receivers[i].Index) + receivers[i].Name + HTMLHelpers.TagHelpers.CreateEndElement("a")
                    ));
                }
            }
            else {
                res.Add((
                    GetEnumerationStringLinks(senders),
                    GetEnumerationStringLinks(receivers)
                ));
            }
        }
        return res;
    }

    public static string GetEnumerationStringLinks(IEnumerable<Person> strlist) {
        var res = string.Empty;
        foreach (var str in strlist) {
            if (str != strlist.First())
                if (str == strlist.Last())
                    res += " und " + (str.Index == "1" ? "" : HTMLHelpers.TagHelpers.CreateElement("a", "", "/HKB/Person/" + str.Index)) + str.Name + (str.Index == "1" ? "" : HTMLHelpers.TagHelpers.CreateEndElement("a"));
                else
                    res += ", " + (str.Index == "1" ? "" :HTMLHelpers.TagHelpers.CreateElement("a", "", "/HKB/Person/" + str.Index)) + str.Name + (str.Index == "1" ? "" : HTMLHelpers.TagHelpers.CreateEndElement("a"));
            else
                res += (str.Index == "1" ? "" : HTMLHelpers.TagHelpers.CreateElement("a", "", "/HKB/Person/" + str.Index)) + str.Name + (str.Index == "1" ? "" : HTMLHelpers.TagHelpers.CreateEndElement("a"));
        }
        return res;
    }
}