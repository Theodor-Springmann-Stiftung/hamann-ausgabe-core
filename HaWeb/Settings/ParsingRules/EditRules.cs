namespace HaWeb.Settings.ParsingRules;
using System.Text;
using System.Web;
using HaWeb.Settings;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.EditState>>)>;

public static class EditRules {
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;

    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "align" && x["pos"] == "center", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ALIGNCENTERCLASS)) ),
        ( ( x, _) => x.Name == "align" && x["pos"] == "right", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ALIGNRIGHTCLASS)) ),
        ( ( x, _) => x.Name == "added", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ADDEDCLASS)) ),
        ( ( x, _) => x.Name == "sal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SALCLASS)) ),
        ( ( x, _) => x.Name == "aq", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.AQCLASS)) ),
        ( ( x, _) => x.Name == "super", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.SUPERCLASS)) ),
        ( ( x, _) => x.Name == "del", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.DELCLASS));
            reader.State.active_del = true;
        }),
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
        ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.HEADERCLASS)) ),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.LEMMACLASS)) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ENTRYCLASS)) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TITLECLASS)) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.BZGCLASS)) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.ZHCLASS)) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.EMPHCLASS)); } ),
        ( ( x, _) => x.Name == "app", (sb, tag, _) => {  sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.APPCLASS)); } ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.EDITREASONCLASS)) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.MARGINALCLASS));
            reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
        }),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.HANDCLASS)) ),
        ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TABLECLASS)) ),
        ( ( x, _) => x.Name == "tab" && !String.IsNullOrWhiteSpace(x["value"]), (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TABCLASS + tag["value"])))
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
        ( ( x, _) => x.Name == "insertion", (sb, tag,_) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "header", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "bzg", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "zh", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "emph", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "app", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "editreason", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "marginal", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "tabs", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "tab", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( ( x, _) => x.Name == "hand", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) )
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( ( x, _) => true, (sb, txt, reader) => {
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, CSSClasses.TEXTCLASS));
            if (reader.State.active_del)
                sb.Append(HttpUtility.HtmlEncode(txt.Value).Replace("–", "<" + DEFAULTELEMENT + " class=\"" + CSSClasses.CROSSEDDASHCLASS + "\">–</" + DEFAULTELEMENT + ">"));
            else
                sb.Append(HttpUtility.HtmlEncode(txt.Value));
            sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
    })};

    public static readonly TagFuncList STagRules = new TagFuncList() {
        ( ( x, _) => x.Name == "line", (sb, tag, _) => sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("br")) )
    };

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( ( _, _) => true, ( sb, txt, reader) => {
            if (reader.State.active_skipwhitespace)
                sb.Append(txt.Value);
            else
                reader.State.active_skipwhitespace = !reader.State.active_skipwhitespace;
    })};
}