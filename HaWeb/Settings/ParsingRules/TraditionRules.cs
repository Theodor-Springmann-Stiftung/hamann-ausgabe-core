namespace HaWeb.Settings.ParsingRules;
using System.Text;
using System.Web;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;

public static class TraditionRules {
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;

    private static readonly string LEMMACLASS = HaWeb.Settings.CSSClasses.LEMMACLASS;
    private static readonly string TITLECLASS = HaWeb.Settings.CSSClasses.TITLECLASS;
    private static readonly string ENTRYCLASS = HaWeb.Settings.CSSClasses.ENTRYCLASS;

    private static readonly string ALIGNCENTERCLASS = HaWeb.Settings.CSSClasses.ALIGNCENTERCLASS;
    private static readonly string ALIGNRIGHTCLASS = HaWeb.Settings.CSSClasses.ALIGNRIGHTCLASS;
    private static readonly string ADDEDCLASS = HaWeb.Settings.CSSClasses.ADDEDCLASS;
    private static readonly string SALCLASS = HaWeb.Settings.CSSClasses.SALCLASS;
    private static readonly string AQCLASS = HaWeb.Settings.CSSClasses.AQCLASS;
    private static readonly string SUPERCLASS = HaWeb.Settings.CSSClasses.SUPERCLASS;
    private static readonly string DELCLASS = HaWeb.Settings.CSSClasses.DELCLASS;
    private static readonly string NRCLASS = HaWeb.Settings.CSSClasses.NRCLASS;
    private static readonly string NOTECLASS = HaWeb.Settings.CSSClasses.NOTECLASS;
    private static readonly string ULCLASS = HaWeb.Settings.CSSClasses.ULCLASS;
    private static readonly string ANCHORCLASS = HaWeb.Settings.CSSClasses.ANCHORCLASS;
    private static readonly string FNCLASS = HaWeb.Settings.CSSClasses.FNCLASS;
    private static readonly string DULCLASS = HaWeb.Settings.CSSClasses.DULCLASS;
    private static readonly string FULCLASS = HaWeb.Settings.CSSClasses.FULCLASS;
    private static readonly string UPCLASS = HaWeb.Settings.CSSClasses.UPCLASS;
    private static readonly string SUBCLASS = HaWeb.Settings.CSSClasses.SUBCLASS;
    private static readonly string TULCLASS = HaWeb.Settings.CSSClasses.TULCLASS;
    private static readonly string HEADERCLASS = HaWeb.Settings.CSSClasses.HEADERCLASS;
    private static readonly string HANDCLASS = HaWeb.Settings.CSSClasses.HANDCLASS;
    private static readonly string TABLECLASS = HaWeb.Settings.CSSClasses.TABLECLASS;
    private static readonly string TABCLASS = HaWeb.Settings.CSSClasses.TABCLASS;
    private static readonly string CROSSEDDASHCLASS = HaWeb.Settings.CSSClasses.CROSSEDDASHCLASS;
    private static readonly string TEXTCLASS = HaWeb.Settings.CSSClasses.TEXTCLASS;

    private static readonly string BZGCLASS = HaWeb.Settings.CSSClasses.BZGCLASS;
    private static readonly string ZHCLASS = HaWeb.Settings.CSSClasses.ZHCLASS;
    private static readonly string EMPHCLASS = HaWeb.Settings.CSSClasses.EMPHCLASS;
    private static readonly string APPCLASS = HaWeb.Settings.CSSClasses.APPCLASS;
    private static readonly string MARGINGALBOXCLASS = HaWeb.Settings.CSSClasses.MARGINGALBOXCLASS;

    // Zeilen:
    private static readonly string ZHLINECLASS = HaWeb.Settings.CSSClasses.ZHLINECLASS;
    private static readonly string FIRSTLINECLASS = HaWeb.Settings.CSSClasses.FIRSTLINECLASS;
    private static readonly string ZHBREAKCLASS = HaWeb.Settings.CSSClasses.ZHBREAKCLASS;
    private static readonly string LINELINECLASS = HaWeb.Settings.CSSClasses.LINELINECLASS;
    private static readonly string LINEINDENTCLASS = HaWeb.Settings.CSSClasses.LINEINDENTCLASS;
    private static readonly string ZHPAGECLASS = HaWeb.Settings.CSSClasses.ZHPAGECLASS;
    private static readonly string ZHLINECOUNTCLASS = HaWeb.Settings.CSSClasses.ZHLINECOUNTCLASS;
    private static readonly string FIRSTPAGECLASS = HaWeb.Settings.CSSClasses.FIRSTPAGECLASS;
    private static readonly string EMPTYLINECLASS = HaWeb.Settings.CSSClasses.EMPTYLINECLASS;
    private static readonly string HIDDENZHLINECOUNT = HaWeb.Settings.CSSClasses.HIDDENZHLINECOUNT;

    // Root-Elemente
    private static readonly string COMMENTCLASS = HaWeb.Settings.CSSClasses.COMMENTCLASS;
    private static readonly string EDITREASONCLASS = HaWeb.Settings.CSSClasses.EDITREASONCLASS;
    private static readonly string SUBSECTIONCLASS = HaWeb.Settings.CSSClasses.SUBSECTIONCLASS;
    private static readonly string TRADITIONCLASS = HaWeb.Settings.CSSClasses.TRADITIONCLASS;
    private static readonly string MARGINALCLASS = HaWeb.Settings.CSSClasses.MARGINALCLASS;
    private static readonly string MARGINALLISTCLASS = HaWeb.Settings.CSSClasses.MARGINALLISTCLASS;
    private static readonly string LETTERCLASS = HaWeb.Settings.CSSClasses.LETTERCLASS;

    // Tradition-spezifische Elemente
    public static readonly string TRADLINECOUNTCLASS = HaWeb.Settings.CSSClasses.TRADLINECOUNTCLASS;
    public static readonly string TRADCOMMENTCOLUMNCLASS = HaWeb.Settings.CSSClasses.TRADCOMMENTCOLUMNCLASS;
    public static readonly string TRADZHTEXTCLASS = HaWeb.Settings.CSSClasses.TRADZHTEXTCLASS;
    public static readonly string TRADZHTEXTBOXCLASS = HaWeb.Settings.CSSClasses.TRADZHTEXTBOXCLASS;

    // Marker-Classes
    private static readonly string EDITMARKERCLASS = HaWeb.Settings.CSSClasses.EDITMARKERCLASS;
    private static readonly string COMMENTMARKERCLASS = HaWeb.Settings.CSSClasses.COMMENTMARKERCLASS;
    private static readonly string HANDMARKERCLASS = HaWeb.Settings.CSSClasses.HANDMARKERCLASS;


    // Parsing Rules for Letters
    // General rules (for the lettertext column, also for parsing the marginals, awa tradtions and editreasons)
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align" && x["pos"] == "center", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ALIGNCENTERCLASS));
            reader.State.mustwrap = (true, true);
        } ),
        ( ( x, _) => x.Name == "align" && x["pos"] == "right", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ALIGNRIGHTCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ADDEDCLASS)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SALCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, AQCLASS)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SUPERCLASS)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, DELCLASS)) ),
        ( ( x, _) => x.Name == "nr", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, NRCLASS)) ),
        ( ( x, _) => x.Name == "note", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, NOTECLASS)) ),
        ( ( x, _) => x.Name == "ul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ULCLASS)) ),
        ( ( x, _) => x.Name == "anchor" && !String.IsNullOrWhiteSpace(x["ref"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ANCHORCLASS)) ),
        ( ( x, _) => x.Name == "fn" && !String.IsNullOrWhiteSpace(x["index"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, FNCLASS)) ),
        ( ( x, _) => x.Name == "dul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, DULCLASS)) ),
        ( ( x, _) => x.Name == "ful", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, FULCLASS)) ),
        ( ( x, _) => x.Name == "up", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, UPCLASS)) ),
        ( ( x, _) => x.Name == "sub", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SUBCLASS)) ),
        ( ( x, _) => x.Name == "tul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TULCLASS)) ),
        ( ( x, _) => x.Name == "header", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, HEADERCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LEMMACLASS)) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ENTRYCLASS)) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS)) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, BZGCLASS)) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHCLASS)) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, EMPHCLASS)); } ),
        ( ( x, _) => x.Name == "app", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, APPCLASS)); } ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SUBSECTIONCLASS, tag["id"])) ),
        ( ( x, _) => x.Name == "kommentar", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, COMMENTCLASS, tag["id"])) ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, EDITREASONCLASS)) ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LETTERCLASS)) ),
        ( ( x, _) => x.Name == "letterTradition", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TRADITIONCLASS)) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, MARGINALCLASS, "m-" + tag["index"]));
            reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
        }),
        ( ( x, _) => x.Name == "tabs", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TABLECLASS));
            // Tabs work with percentages, so we need a static width of the conttainer:
            reader.State.minwidth = true;
        } ),
        ( ( x, _) => x.Name == "tab" && !String.IsNullOrWhiteSpace(x["value"]), (sb, tag, reader) => {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TABCLASS + tag["value"]));
        }),
        ( ( x, _) => x.Name == "edit" && !String.IsNullOrWhiteSpace(x["ref"]), (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, EDITMARKERCLASS, "ea-" + tag["ref"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, HANDMARKERCLASS, "ha-" + tag["ref"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, HANDCLASS));
        }),

         // Tradition specific:
        ( ( x, _) => x.Name == "app", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, APPCLASS)); } ),
        ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TRADZHTEXTCLASS));
            reader.State.active_trad = true;
        })
    };

    public static readonly TagFuncList CTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align", (sb, tag, reader) =>  {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, reader) => {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "nr", (sb, tag, _) =>  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "note", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "ul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "anchor", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "fn", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "dul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "up", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "ful", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "sub", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "tul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "header", (sb, tag, reader) => {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "app", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "kommentar", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "letterTradition", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "tab", (sb, tag, reader) => {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        
        // Tradition specifics
        ( ( x, _) => x.Name == "app", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.active_trad = false;
        })
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( ( x, _) => true, (sb, txt, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TEXTCLASS));
            if (reader.OpenTags.Where(x => x.Name == "del").Any())
                sb.Append(HttpUtility.HtmlEncode(txt.Value).Replace("–", "<" + DEFAULTELEMENT + " class=\"" + CROSSEDDASHCLASS + "\">–</" + DEFAULTELEMENT + ">"));
            else
                sb.Append(HttpUtility.HtmlEncode(txt.Value));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
    })};

    public static readonly TagFuncList STagRules = new TagFuncList() {
        ( (x, _) => x.Name == "page", (sb, tag, reader) => reader.State.currpage = tag["index"] ),
        ( (x, _) => x.Name == "line", (sb, tag, reader) => {
            if(!String.IsNullOrWhiteSpace(tag["tab"]) || !String.IsNullOrWhiteSpace(tag["type"])) {
                reader.State.mustwrap = (false, true);
            }

            // Tradition specific: only if in zhtext
            if (reader.State.active_trad) {
                // This is NOT the beginning of the text, so we set a br, and then, linecount
                if(reader.State.currline != "-1") {
                    if (!reader.State.mustwrap.Item2)
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", ZHBREAKCLASS));
                    else
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br"));
                    reader.State.mustwrap = (false, false);
                    
                    // Linecount
                    if(!String.IsNullOrWhiteSpace(tag["index"])) {
                        reader.State.currline = tag["index"];
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHLINECOUNTCLASS, reader.State.currpage + "-" + reader.State.currline));
                    
                        // Fall 1: Neue Seite
                        if (reader.State.currline == "1") {
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHPAGECLASS, ""));
                            sb.Append("S.&nbsp;" + reader.State.currpage);
                        } 

                        // Fall 2: Neue Zeile, teilbar durch 5
                        else if (Int32.TryParse(tag["index"], out var _) && Int32.Parse(tag["index"]) % 5 == 0) {
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHLINECLASS, ""));
                            sb.Append(tag["index"]);
                        } 
                        
                        // Fall 3: Neue Zeile, nicht teilbar durch 5, deswegen versteckt
                        else {
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHLINECLASS + " " + HIDDENZHLINECOUNT, ""));
                            sb.Append(tag["index"]);
                        }

                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    }
                } else if (reader.State.currline == "-1" && !String.IsNullOrWhiteSpace(tag["index"])) {
                    reader.State.Startline = tag["index"];
                    reader.State.currline = tag["index"];
                    
                    // Tradition specifics: the first linecount must be shown
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHLINECOUNTCLASS + " " + FIRSTLINECLASS, reader.State.currpage + "-" + reader.State.currline));
                    sb.Append("S. " + reader.State.currpage + "&thinsp;/&thinsp;" + reader.State.currline);
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                }

                // Marginalien, only for lines with a linenumber
                if(reader.State.Marginals != null && !String.IsNullOrWhiteSpace(tag["index"])) {
                    var margs = reader.State.Marginals.Where(x => x.Page == reader.State.currpage && x.Line == reader.State.currline);
                    if (margs != null && margs.Any())
                    {
                        if(reader.State.ParsedMarginals == null) reader.State.ParsedMarginals = new List<(string, string, string)>();
                        var sb2 = new StringBuilder();
                        margs = margs.OrderBy(x => Int32.Parse(x.Index));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, COMMENTMARKERCLASS, "ma-" + reader.State.currpage + "-" + reader.State.currline));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, MARGINGALBOXCLASS));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, MARGINALLISTCLASS));
                        foreach (var marginal in margs)
                        {
                            // In Marginal, the Root-Element (<marginal>) is somehow parsed,
                            // so we don't need to enclose it in a seperate div.
                            var rd = reader.State.ReaderService.RequestStringReader(marginal.Element);
                            new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>(reader.State, rd, sb, TraditionRules.OTagRules, null, TraditionRules.CTagRules, TraditionRules.TextRules, TraditionRules.WhitespaceRules);
                            new HaWeb.HTMLHelpers.LinkHelper(reader.State.Lib, rd, sb, false);
                            new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>(reader.State, rd, sb2, TraditionRules.OTagRules, null, TraditionRules.CTagRules, TraditionRules.TextRules, TraditionRules.WhitespaceRules);
                            new HaWeb.HTMLHelpers.LinkHelper(reader.State.Lib, rd, sb2, false);
                            rd.Read();
                        }
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        reader.State.ParsedMarginals.Add((reader.State.currpage, reader.State.currline, sb2.ToString()));
                    }
                }

                // Line type=line
                if(tag["type"] == "line") {
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LINELINECLASS));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    reader.State.mustwrap = (false, true);
                }

                // Line tab=
                if(!String.IsNullOrWhiteSpace(tag["tab"])) {
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LINEINDENTCLASS + tag["tab"]));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    if (tag["tab"] != "1") reader.State.mustwrap = (false, true);
                }
            }

            // Tradition specific: only do this if not in ZHText
            if (!reader.State.active_trad) {
                sb.Append("<br>");
            }
        }
    )};

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( ( _, _) => true, ( sb, txt, reader) => {
            if (reader.State.active_skipwhitespace)
                sb.Append(txt.Value);
            else
                reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
    })};
}