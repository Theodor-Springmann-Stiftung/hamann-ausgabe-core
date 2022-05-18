namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

// Type aliasses for incredible long types
using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, BriefeHelper, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, BriefeHelper>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, BriefeHelper, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, BriefeHelper>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, BriefeHelper, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, BriefeHelper>)>;

public class BriefeHelper
{
    // Input
    private protected ILibrary Lib;
    private protected IReaderService ReaderService;
    private protected Meta Meta;

    private protected Letter? Letter;
    private protected Tradition? Tradition;
    private protected ImmutableList<Hand>? Hands;
    private protected ImmutableList<Editreason>? EditReasons;
    private protected ImmutableList<Marginal>? Marginals;

    // State
    private protected string currline = "-1";
    private protected string currpage = "";
    private protected string oldpage = "";
    private protected int commid = 1;
    private protected bool active_firstedit = true;
    private protected bool active_trad = false;
    private protected bool active_skipwhitespace = true;
    private protected bool active_del = false;
    private protected List<string> handstrings = new List<string>();

    // Parsing-Combinations
    private protected StringBuilder sb_lettertext = new StringBuilder();    // Hauptext
    private protected StringBuilder sb_linecount = new StringBuilder();     // Linke Spalte (Zeilenzählung)
    private protected StringBuilder sb_marginals = new StringBuilder();     // Rechte Spalte (Kommentare)
    private protected StringBuilder sb_tradition = new StringBuilder();     // Überlieferung
    private protected StringBuilder sb_trad_zhtext = new StringBuilder();   // Überlieferung, ZHText
    private protected StringBuilder sb_trad_left = new StringBuilder();     // Überlieferung ZHText linke Spalte (zeilenzählung)
    private protected StringBuilder sb_trad_right = new StringBuilder();    // Überlieferung ZHText rechte Spalte (Kommentare)
    private protected StringBuilder sb_edits = new StringBuilder();         // Edits

    private protected IReader? rd_lettertext;
    private protected IReader? rd_tradition;

    // Parsing Rules
    // General rules (for the lettertext column, also for parsing the marginals, awa tradtions and editreasons)
    private static readonly TagFuncList OTag_Funcs = new TagFuncList() {
        ( ( x, _) => x.Name == "align" && x["pos"] == "center", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "align center")) ),
        ( ( x, _) => x.Name == "align" && x["pos"] == "right", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "align right")) ),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "added")) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "sal")) ),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "aq")) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "super")) ),
        ( ( x, _) => x.Name == "del", (sb, tag, bh) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "del"));
            bh.active_del = true;
            } ),
        ( ( x, _) => x.Name == "nr", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "nr")) ),
        ( ( x, _) => x.Name == "note", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "note")) ),
        ( ( x, _) => x.Name == "ul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "ul")) ),
        ( ( x, _) => x.Name == "anchor" && !String.IsNullOrWhiteSpace(x["ref"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "anchor")) ),
        ( ( x, _) => x.Name == "fn" && !String.IsNullOrWhiteSpace(x["index"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "footnote")) ),
        ( ( x, _) => x.Name == "dul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "dul")) ),
        ( ( x, _) => x.Name == "ful", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "ful")) ),
        ( ( x, _) => x.Name == "up", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "up")) ),
        ( ( x, _) => x.Name == "sub", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("sub")) ),
        ( ( x, _) => x.Name == "tul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "tul")) ),
        ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "header")) ),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "lemma")) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "entry")) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "title")) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "bzg")) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "zh")) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("em")); } ),
        ( ( x, _) => x.Name == "app", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "app")); } ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "subcomment", tag["id"])) ),
        ( ( x, _) => x.Name == "kommentar", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "comment", tag["id"])) ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "editreason")) ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "letter")) ),
        ( ( x, _) => x.Name == "letterTradition", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "tradition")) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, bh) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "marginal"));
                bh.active_skipwhitespace = !bh.active_skipwhitespace;
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "hand"));
            } ),
            ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "htable")) ),
            ( ( x, _) => x.Name == "tab" && !String.IsNullOrWhiteSpace(x["value"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "htab htab-" + tag["value"])))
    };

    public BriefeHelper(ILibrary lib, IReaderService readerService, Meta meta, Letter? letter, Tradition? tradition, ImmutableList<Hand>? hands, ImmutableList<Editreason> editreasons, ImmutableList<Marginal> marginals)
    {
        Lib = lib;
        ReaderService = readerService;
        Letter = letter;
        Meta = meta;
        Tradition = tradition;
        Hands = hands;
        EditReasons = editreasons;
        Marginals = marginals;

        initState();
    }

    private void initState()
    {
        rd_lettertext = Letter != null && !String.IsNullOrWhiteSpace(Letter.Element) ? ReaderService.RequestStringReader(Letter.Element) : null;
        rd_tradition = Tradition != null && !String.IsNullOrWhiteSpace(Tradition.Element) ? ReaderService.RequestStringReader(Tradition.Element) : null;
        if (Meta.ZH != null)
        {
            currpage = Meta.ZH.Page;
        }
        if (Hands != null)
        {
            foreach (var hand in Hands.OrderBy(x => x.StartPage.Length).ThenBy(x => x.StartPage).ThenBy(x => x.StartLine.Length).ThenBy(x => x.StartLine))
            {
                var currstring = hand.StartPage + "/" + hand.StartLine;
                if (hand.StartPage != hand.EndPage)
                {
                    currstring += "–" + hand.EndPage + "/" + hand.EndLine;
                }
                else
                {
                    if (hand.StartLine != hand.EndLine)
                    {
                        currstring += "–" + hand.EndLine;
                    }
                }
                if (Lib.HandPersons.Where(x => x.Key == hand.Person).Any())
                {
                    currstring += " " + Lib.HandPersons.Where(x => x.Key == hand.Person).FirstOrDefault().Value.Name;
                    handstrings.Add(currstring);
                }
            }
        }
    }

    public void CreateHTML()
    {
        var CTag_Funcs = new TagFuncList() {
            ( ( x, _) => x.Name == "align", (sb, tag, _) =>  {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
             } ),
            ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "sal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "del", (sb, tag, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                active_del = false;
             } ),
            ( ( x, _) => x.Name == "nr", (sb, tag, _) =>  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "note", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "ul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "anchor", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "fn", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "dul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "up", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "ful", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "sub", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("sub")) ),
            ( ( x, _) => x.Name == "tul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "emph", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("em")) ),
            ( ( x, _) => x.Name == "app", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span")) ),
            ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "kommentar", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "letterTradition", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "marginal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "tab", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div")) ),
            ( ( x, _) => x.Name == "hand", (sb, tag, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
             } )
        };

        var Text_Funcs = new TextFuncList() {
            ( ( x, _) => true, ( sb, txt, _) => {
                if (active_del)
                    sb.Append(txt.Value.Replace("–", "<span class=\"diagdel\">–</span>"));
                else
                    sb.Append(txt.Value);
             } )
        };

        var Text_Funcs_Tagging = new TextFuncList() {
            ( ( _, _) => true, ( sb, txt, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "ntext"));
                sb.Append(txt.Value);
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
             } )
        };

        var STag_Funcs = new TagFuncList() {
            ( ( x, _) => x.Name == "line", (sb, tag, _) => {
                if(currline != "-1") sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "ZHBreak"));
                if(tag["type"] == "line") sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("hr", "lineline"));
            } ),
            ( ( x, _) => x.Name == "line" && !String.IsNullOrWhiteSpace(x["tab"]), (sb, tag, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "tab-" + tag["tab"]));
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            } )
        };

        var Whitespace_Funcs = new WhitespaceFuncList() {
            ( ( _, _) => true, ( sb, txt, _) => {
                if (active_skipwhitespace)
                    sb.Append(txt.Value);
                else
                    active_skipwhitespace = !active_skipwhitespace;
            })
        };

        // Rules for the left sidebar
        var STag_Funcs_LEFT = new TagFuncList() {
            ( ( x, _) => x.Name == "line", (sb, tag, _) => {
                if(currline != "-1") {
                    if (currpage == oldpage)
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "", currpage + "-" + currline));
                    else {
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "", oldpage + "-" + currline));
                        oldpage = currpage;
                    }
                }
                else {
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "zhpage firstpage", currpage + "-" + tag["index"]));
                    sb.Append("S." + "&nbsp;");
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                    if (tag["index"] != "1")
                        sb.Append(currpage + "&thinsp;/&thinsp;" + tag["index"]);
                    else
                        sb.Append(currpage);
                    oldpage = currpage;
                }
            }),
            ( ( x, _) => x.Name == "line", (sb, tag, _) => { if(currline != "-1" && Int32.TryParse(tag["index"], out var _) && Int32.Parse(tag["index"]) % 5 == 0) { sb.Append(tag["index"]); } } ),
            ( ( x, _) => x.Name == "line" && x["index"] == "1" && currline != "-1", (sb, tag, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "zhpage", ""));
                sb.Append("S. " + currpage);
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            }),
            ( ( x, _) => x.Name == "line", (sb, tag, _) => { currline = tag["index"]; } ),
            ( ( x, _) => x.Name == "page", (sb, tag, _) =>  { currpage = tag["index"]; } )
        };

        // Rules for the right sidebar
        var STag_Funcs_RIGHT = new TagFuncList() {
            ( ( x, _) => x.Name == "line", (sb, tag, _) => {
                if(currline != "-1" && Marginals != null) {
                    var margs = Marginals.Where(x => x.Page == currpage && x.Line == currline);
                    if (margs != null && margs.Any())
                    {
                        margs = margs.OrderBy(x => Int32.Parse(x.Index));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "commBox", commid.ToString()));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "commselector"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("button", "button"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("button"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "comment"));
                        foreach (var marginal in margs)
                        {
                            var rd = ReaderService.RequestStringReader(marginal.Element);
                            new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd, sb, OTag_Funcs, null, CTag_Funcs, Text_Funcs_Tagging, Whitespace_Funcs);
                            new HaWeb.HTMLHelpers.LinkHelper(Lib, rd, sb, false);
                            rd.Read();
                        }
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br"));
                    }
                    else
                    {
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "emptyline"));
                    }
                }
                commid++;
            }
        )};

        // Rules for traditions
        var OTag_Funcs_Trad = new TagFuncList() {
        ( ( x, _) => x.Name == "app", (sb, tag, _) => {  if (!active_firstedit) { sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br")); } else { active_firstedit = false; } } ),
        ( ( x, _) => x.Name == "ZHText", (sb, tag, _) => {
                sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "row zhtext"));
                sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "trad-text col order-2 letterbox"));
                sb_trad_left = new StringBuilder();
                sb_trad_right = new StringBuilder();
                currline = "-1";
                currpage = "";
                active_trad = true;
                sb_trad_left.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "trad-linecount countbox nnumber d-none d-lg-block order-1"));
                sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "commentColumn trad-comm col-4 d-none d-lg-block order-3"));
                sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "emptyline"));
                if (rd_tradition != null) {
                    new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_tradition, sb_trad_left, null, STag_Funcs_LEFT);
                    new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_tradition, sb_trad_right, null, STag_Funcs_RIGHT);
                }
            } )
        };

        var CTag_Funcs_Trad = new TagFuncList() {
        ( ( x, _) => x.Name == "ZHText", (sb, tag, _) => {
                sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                sb_trad_left.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                sb_tradition.Append(sb_trad_left.ToString());
                sb_tradition.Append(sb_trad_right.ToString());
                sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
                active_trad = false;
            } )
        };

        var STags_Funcs_TRAD = new TagFuncList() {
            ( ( x, _) => x.Name == "line", (sb, tag, _) => { if(currline != "-1" || !active_trad) sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "ZHBreak")); } ),
            ( ( x, _) => x.Name == "line" && !String.IsNullOrWhiteSpace(x["tab"]), (sb, tag, _) => {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "tab-" + tag["tab"]));
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            } )
        };

        // Rules for Edits:
        var STags_Funcs_EDIT = new TagFuncList() {
            ( ( x, _) => x.Name == "line", (sb, tag, _) => sb.Append("&emsp;") )
        };

        string HandleEdit(Editreason edit)
        {
            sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "edit"));
            sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "pageline"));
            var currstring = edit.StartPage + "/" + edit.StartLine;
            if (edit.StartPage != edit.EndPage)
            {
                currstring += "–" + edit.EndPage + "/" + edit.EndLine;
            }
            else
            {
                if (edit.StartLine != edit.EndLine)
                {
                    currstring += "–" + edit.EndLine;
                }
            }
            sb_edits.Append(currstring + "&emsp;");
            sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            if (!String.IsNullOrWhiteSpace(edit.Reference))
            {
                var sb2 = new StringBuilder();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                var rd = ReaderService.RequestStringReader(edit.Reference);
                new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd, sb2, OTag_Funcs, null, CTag_Funcs, Text_Funcs, Whitespace_Funcs);
                rd.Read();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                if ((edit.StartPage != edit.EndPage || edit.StartLine != edit.EndLine) && XElement.Parse(sb2.ToString()).Value.ToString().Length >= 60)
                {
                    var text = XElement.Parse(sb2.ToString()).Value.ToString();
                    text = text.ToString().Split(' ').Take(1).First() + " [&#x2026;] " + text.ToString().Split(' ').TakeLast(1).First();
                    sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                    sb_edits.Append(text);
                    sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                }
                else
                    sb_edits.Append(sb2);
            }
            if (!String.IsNullOrWhiteSpace(edit.Element))
            {
                sb_edits.Append("&emsp;");
                sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "corrections"));
                var rd = ReaderService.RequestStringReader(edit.Element);
                new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd, sb_edits, OTag_Funcs, STags_Funcs_EDIT, CTag_Funcs, Text_Funcs, Whitespace_Funcs);
                rd.Read();
                sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            }
            sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
            return sb_edits.ToString();
        }

        // Actual parsing
        if (Letter != null && Letter.Element != null && !String.IsNullOrWhiteSpace(Letter.Element) && rd_lettertext != null)
        {
            new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_lettertext, sb_lettertext, OTag_Funcs, STag_Funcs, CTag_Funcs, Text_Funcs, Whitespace_Funcs);
            new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_lettertext, sb_linecount, null, STag_Funcs_LEFT);

            if (Marginals != null && Marginals.Any())
            {
                new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_lettertext, sb_marginals, null, STag_Funcs_RIGHT);
            }
            rd_lettertext.Read();
        }

        if (Tradition != null && !String.IsNullOrWhiteSpace(Tradition.Element) && rd_tradition != null)
        {
            new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_tradition, sb_tradition, OTag_Funcs_Trad, null, CTag_Funcs_Trad, null, null);
            new HaWeb.HTMLHelpers.GenericXMLHelper<BriefeHelper>(this, rd_tradition, sb_tradition, OTag_Funcs, STags_Funcs_TRAD, CTag_Funcs, Text_Funcs, Whitespace_Funcs);
            new HaWeb.HTMLHelpers.LinkHelper(Lib, rd_tradition, sb_tradition);
            rd_tradition.Read();
        }

        if (EditReasons != null && EditReasons.Any())
        {
            foreach (var edit in EditReasons)
            {
                HandleEdit(edit);
            }
        }
    }
}