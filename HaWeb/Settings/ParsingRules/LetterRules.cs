namespace HaWeb.Settings.ParsingRules;
using System.Text;
using System.Web;
using HaWeb.Settings;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>>)>;

// TODO: stringbuilder als Rückgabeparameter des XMHelpers ist eigentlich auch Part vom State
public class LetterRules {
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;

    // Parsing Rules for Letters
    // General rules (for the lettertext column, also for parsing the marginals, awa tradtions and editreasons)
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align" && x["pos"] == "center", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ALIGNCENTERCLASS));
            reader.State.mustwrap = (true, true);
        } ),
        ( ( x, _) => x.Name == "align" && x["pos"] == "right", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ALIGNRIGHTCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ADDEDCLASS)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SALCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.AQCLASS)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SUPERCLASS)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.DELCLASS)) ),
        ( ( x, _) => x.Name == "nr", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.NRCLASS)) ),
        ( ( x, _) => x.Name == "note", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.NOTECLASS)) ),
        ( ( x, _) => x.Name == "ul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ULCLASS)) ),
        ( ( x, _) => x.Name == "anchor" && !String.IsNullOrWhiteSpace(x["ref"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ANCHORCLASS)) ),
        ( ( x, _) => x.Name == "fn" && !String.IsNullOrWhiteSpace(x["index"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.FNCLASS)) ),
        ( ( x, _) => x.Name == "dul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.DULCLASS)) ),
        ( ( x, _) => x.Name == "ful", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.FULCLASS)) ),
        ( ( x, _) => x.Name == "up", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.UPCLASS)) ),
        ( ( x, _) => x.Name == "sub", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SUBCLASS)) ),
        ( ( x, _) => x.Name == "tul", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TULCLASS)) ),
        ( ( x, _) => x.Name == "insertion", (sb, tag,_) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.INSERTIONCLASS)) ),
        ( ( x, _) => x.Name == "header", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.HEADERCLASS));
            reader.State.mustwrap = (true, true);
        }),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.LEMMACLASS)) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ENTRYCLASS)) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TITLECLASS)) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.BZGCLASS)) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHCLASS)) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.EMPHCLASS)); } ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SUBSECTIONCLASS, tag["id"])) ),
        ( ( x, _) => x.Name == "kommentar", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.COMMENTCLASS, tag["id"])) ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.EDITREASONCLASS)) ),
        ( ( x, _) => x.Name == "subsection", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.LETTERCLASS)) ),
        ( ( x, _) => x.Name == "letterTradition", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TRADITIONCLASS)) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.MARGINALCLASS, "m-" + tag["index"]));
            reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
        }),
        ( ( x, _) => x.Name == "tabs", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TABLECLASS));
            // Tabs work with percentages, so we need a static width of the conttainer:
            reader.State.minwidth = true;
        } ),
        ( ( x, _) => x.Name == "tab" && !String.IsNullOrWhiteSpace(x["value"]), (sb, tag, reader) => {
            reader.State.mustwrap = (true, true);
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TABCLASS + tag["value"]));
        }),
        ( ( x, _) => x.Name == "edit" && !String.IsNullOrWhiteSpace(x["ref"]), (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.EDITMARKERCLASS, "ea-" + tag["ref"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.HANDMARKERCLASS, "ha-" + tag["ref"]));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.HANDCLASS));
        }),

        // Tradition specific:
        ( ( x, _) => x.Name == "app", (sb, tag, reader) => {  
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.APPCLASS));
            reader.State.activelinecount = false;
        } ),
        ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TRADZHTEXTCLASS));
            reader.State.activelinecount = true;
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
        ( ( x, _) => x.Name == "insertion", (sb, tag,_) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
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

        // Tradition specific:
        ( ( x, _) => x.Name == "app", (sb, tag, reader) => {  
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.activelinecount = true;
        } ),
        ( ( x, _) => x.Name == "ZHText", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
            reader.State.activelinecount = false;
        })
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( ( x, _) => true, (sb, txt, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TEXTCLASS));
            if (reader.OpenTags.Where(x => x.Name == "del").Any())
                sb.Append(HttpUtility.HtmlEncode(txt.Value).Replace("–", "<" + DEFAULTELEMENT + " class=\"" + CSSClasses.CROSSEDDASHCLASS + "\">–</" + DEFAULTELEMENT + ">"));
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

            // Line count, do if in counted lines
            if(reader.State.activelinecount) {
                // This is NOT the beginning of the text, so we set a br, and then, linecount
                if(reader.State.currline != "-1") {
                    if (!reader.State.mustwrap.Item2)
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br", CSSClasses.ZHBREAKCLASS));
                    else
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br"));
                    reader.State.mustwrap = (false, false);
                    
                    // Linecount
                    if(!String.IsNullOrWhiteSpace(tag["index"])) {
                        reader.State.currline = tag["index"];
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHLINECOUNTCLASS, reader.State.currpage + "-" + reader.State.currline));
                    
                        // Fall 1: Neue Seite
                        if (reader.State.pagebreak == true) {
                            reader.State.pagebreak = false;
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHPAGECLASS, ""));
                            sb.Append("S.&nbsp;" + reader.State.currpage);
                        } 

                        // Fall 2: Neue Zeile, teilbar durch 5
                        else if (Int32.TryParse(tag["index"], out var _) && Int32.Parse(tag["index"]) % 5 == 0) {
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHLINECLASS, ""));
                            sb.Append(tag["index"]);
                        } 
                        
                        // Fall 3: Neue Zeile, nicht teilbar durch 5, deswegen versteckt
                        else {
                            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHLINECLASS + " " + CSSClasses.HIDDENZHLINECOUNT, ""));
                            sb.Append(tag["index"]);
                        }

                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    }
                } else if (reader.State.currline == "-1" && !String.IsNullOrWhiteSpace(tag["index"])) {
                    reader.State.Startline = tag["index"];
                    reader.State.currline = tag["index"];
                    reader.State.pagebreak = false;
                }

                // Marginalien, only for lines with a linenumber
                if(reader.State.Marginals != null && !String.IsNullOrWhiteSpace(tag["index"])) {
                    var margs = reader.State.Marginals.Where(x => x.Page == reader.State.currpage && x.Line == reader.State.currline);
                    if (margs != null && margs.Any())
                    {
                        if(reader.State.ParsedMarginals == null) reader.State.ParsedMarginals = new List<(string, string, string)>();
                        var sb2 = new StringBuilder();
                        margs = margs.OrderBy(x => Int32.Parse(x.Index));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.COMMENTMARKERCLASS, "ma-" + reader.State.currpage + "-" + reader.State.currline));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.MARGINGALBOXCLASS));
                        sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.MARGINALLISTCLASS));
                        foreach (var marginal in margs)
                        {
                            // In Marginal, the Root-Element (<marginal>) is somehow parsed,
                            // so we don't need to enclose it in a seperate div.
                            var rd = reader.State.ReaderService.RequestStringReader(marginal.Element);
                            new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>(reader.State, rd, sb, LetterRules.OTagRules, null, LetterRules.CTagRules, LetterRules.TextRules, LetterRules.WhitespaceRules);
                            new HaWeb.HTMLHelpers.LinkHelper(reader.State.Lib, rd, sb, false);
                            new HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LetterState>(reader.State, rd, sb2, LetterRules.OTagRules, null, LetterRules.CTagRules, LetterRules.TextRules, LetterRules.WhitespaceRules);
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
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.LINELINECLASS));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    reader.State.mustwrap = (false, true);
                }

                // Line tab=
                if(!String.IsNullOrWhiteSpace(tag["tab"])) {
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.LINEINDENTCLASS + tag["tab"]));
                    sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                    if (tag["tab"] != "1") reader.State.mustwrap = (false, true);
                }
            }

            if (!reader.State.activelinecount) {
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