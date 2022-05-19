namespace HaWeb.Settings.ParsingRules;
using System.Text;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>>)>;

public static class TraditionRules
{
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
    private static readonly string ZHBREAKCLASS = HaWeb.Settings.CSSClasses.ZHBREAKCLASS;
    private static readonly string LINELINECLASS = HaWeb.Settings.CSSClasses.LINELINECLASS;
    private static readonly string LINEINDENTCLASS = HaWeb.Settings.CSSClasses.LINEINDENTCLASS;
    private static readonly string ZHPAGECLASS = HaWeb.Settings.CSSClasses.ZHBREAKCLASS;
    private static readonly string FIRSTPAGECLASS = HaWeb.Settings.CSSClasses.FIRSTPAGECLASS;
    private static readonly string EMPTYLINECLASS = HaWeb.Settings.CSSClasses.EMPTYLINECLASS;

    // Root-Elemente
    private static readonly string COMMENTCLASS = HaWeb.Settings.CSSClasses.COMMENTCLASS;
    private static readonly string EDITREASONCLASS = HaWeb.Settings.CSSClasses.EDITREASONCLASS;
    private static readonly string SUBSECTIONCLASS = HaWeb.Settings.CSSClasses.SUBSECTIONCLASS;
    private static readonly string TRADITIONCLASS = HaWeb.Settings.CSSClasses.TRADITIONCLASS;
    private static readonly string MARGINALCLASS = HaWeb.Settings.CSSClasses.MARGINALCLASS;
    private static readonly string LETTERCLASS = HaWeb.Settings.CSSClasses.LETTERCLASS;

    // Tradition-spezifische Elemente
    public static readonly string TRADLINECOUNTCLASS = HaWeb.Settings.CSSClasses.TRADLINECOUNTCLASS;
    public static readonly string TRADCOMMENTCOLUMNCLASS = HaWeb.Settings.CSSClasses.TRADCOMMENTCOLUMNCLASS;
    public static readonly string TRADZHTEXTCLASS = HaWeb.Settings.CSSClasses.TRADZHTEXTCLASS;
    public static readonly string TRADZHTEXTBOXCLASS = HaWeb.Settings.CSSClasses.TRADZHTEXTBOXCLASS;

    // Parsing Rules for Letters
    // General rules (for the lettertext column, also for parsing the marginals, awa tradtions and editreasons)
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align" && x["pos"] == "center", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ALIGNCENTERCLASS)) ),
        ( ( x, _) => x.Name == "align" && x["pos"] == "right", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ALIGNRIGHTCLASS)) ),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ADDEDCLASS)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SALCLASS)) ),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, AQCLASS)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, SUPERCLASS)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, "del"));
            reader.State.active_del = true;
        }),
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
        ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, HEADERCLASS)) ),
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
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, MARGINALCLASS));
            reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, HANDCLASS)) ),
        ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TABLECLASS)) ),
        ( ( x, _) => x.Name == "tab" && !String.IsNullOrWhiteSpace(x["value"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TABCLASS + tag["value"])))
    };

    public static readonly TagFuncList CTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align", (sb, tag, _) =>  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.active_del = false; // TODO SMTH IS FISHY HERE!
        }),
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
        ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
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
        ( ( x, _) => x.Name == "tab", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) )
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( ( x, _) => true, (sb, txt, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TEXTCLASS));
            if (reader.State.active_del)
                sb.Append(txt.Value.Replace("–", "<" + DEFAULTELEMENT + " class=\"" + CROSSEDDASHCLASS + "\">–</" + DEFAULTELEMENT + ">"));
            else
                sb.Append(txt.Value);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
    })};

    public static readonly TagFuncList STagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => {
            if(reader.State.currline != "-1") sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", ZHBREAKCLASS));
            if(tag["type"] == "line") sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LINELINECLASS));
        } ),
        ( ( x, _) => x.Name == "line" && !String.IsNullOrWhiteSpace(x["tab"]), (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LINEINDENTCLASS + tag["tab"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
    })};

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( ( _, _) => true, ( sb, txt, reader) => {
            if (reader.State.active_skipwhitespace)
                sb.Append(txt.Value);
            else
                reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
    })};


    // Rules for the left sidebar
    public static readonly TagFuncList STagRulesLineCount = new TagFuncList() {
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => {
            if(reader.State.currline != "-1") {
                if (reader.State.currpage == reader.State.oldpage)
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "", reader.State.currpage + "-" + reader.State.currline));
                else {
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", "", reader.State.oldpage + "-" + reader.State.currline));
                    reader.State.oldpage = reader.State.currpage;
                }
            }
            else {
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHPAGECLASS + " " + FIRSTPAGECLASS, reader.State.currpage + "-" + tag["index"]));
                sb.Append("S." + "&nbsp;");
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                if (tag["index"] != "1")
                    sb.Append(reader.State.currpage + "&thinsp;/&thinsp;" + tag["index"]);
                else
                    sb.Append(reader.State.currpage);
                reader.State.oldpage = reader.State.currpage;
            }
        }),
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => {
            if(reader.State.currline != "-1" && Int32.TryParse(tag["index"], out var _) && Int32.Parse(tag["index"]) % 5 == 0)
             sb.Append(tag["index"]);
        }),
        ( ( x, reader) => x.Name == "line" && x["index"] == "1" && reader.State.currline != "-1", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ZHPAGECLASS, ""));
            sb.Append("S. " + reader.State.currpage);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => { reader.State.currline = tag["index"]; } ),
        ( ( x, _) => x.Name == "page", (sb, tag, reader) =>  { reader.State.currpage = tag["index"]; } )
    };


    // Rules for the right sidebar
    public static readonly TagFuncList STagRulesMarginals = new TagFuncList() {
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => {
            if(reader.State.currline != "-1" && reader.State.Marginals != null) {
                var margs = reader.State.Marginals.Where(x => x.Page == reader.State.currpage && x.Line == reader.State.currline);
                if (margs != null && margs.Any())
                {
                    margs = margs.OrderBy(x => Int32.Parse(x.Index));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, MARGINGALBOXCLASS, reader.State.commid.ToString()));
                    foreach (var marginal in margs)
                    {
                        var rd = reader.State.ReaderService.RequestStringReader(marginal.Element);
                        new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>(reader.State, rd, sb, TraditionRules.OTagRules, null, TraditionRules.CTagRules, TraditionRules.TextRules, TraditionRules.WhitespaceRules);
                        new HaWeb.HTMLHelpers.LinkHelper(reader.State.Lib, rd, sb, false);
                        rd.Read();
                    }
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br"));
                }
                else
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", EMPTYLINECLASS));
            }
            reader.State.commid++;
    })};

    public static readonly TagFuncList OTagRulesInitial = new TagFuncList() {
    ( ( x, _) => x.Name == "app", (sb, tag, reader) => {
        if (!reader.State.active_firstedit)
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br"));
        else
            reader.State.active_firstedit = false;
    }),
    ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, "row zhtext"));
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, "trad-text col order-2 letterbox"));
            reader.State.sb_trad_left = new StringBuilder();
            reader.State.sb_trad_right = new StringBuilder();
            reader.State.currline = "-1";
            reader.State.currpage = "";
            reader.State.active_trad = true;
            reader.State.sb_trad_left.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, "trad-linecount countbox nnumber d-none d-lg-block order-1"));
            reader.State.sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, "commentColumn trad-comm col-4 d-none d-lg-block order-3"));
            reader.State.sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", EMPTYLINECLASS));
            if (reader.State.rd_tradition != null) {
                new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>(reader.State, reader.State.rd_tradition, reader.State.sb_trad_left, null, TraditionRules.STagRulesLineCount);
                new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.TraditionState>(reader.State, reader.State.rd_tradition, reader.State.sb_trad_right, null, TraditionRules.STagRulesMarginals);
            }
    })};

    public static readonly TagFuncList CTagRulesInitial = new TagFuncList() {
        ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.sb_trad_left.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.sb_trad_right.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.sb_tradition.Append(reader.State.sb_trad_left.ToString());
            reader.State.sb_tradition.Append(reader.State.sb_trad_right.ToString());
            reader.State.sb_tradition.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.active_trad = false;
    })};

    public static readonly TagFuncList STagRulesInitial = new TagFuncList() {
        ( ( x, _) => x.Name == "line", (sb, tag, reader) => {
            if(reader.State.currline != "-1" || !reader.State.active_trad)
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", ZHBREAKCLASS));
        }),
        ( ( x, _) => x.Name == "line" && !String.IsNullOrWhiteSpace(x["tab"]), (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TABCLASS + tag["tab"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        })};
}