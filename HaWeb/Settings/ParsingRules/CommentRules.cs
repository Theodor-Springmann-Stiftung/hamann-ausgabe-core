namespace HaWeb.Settings.ParsingRules;
using System.Text;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.CommentState>>)>;

public static class CommentRules {
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;
    private static readonly string LEMMACLASS = HaWeb.Settings.CSSClasses.LEMMACLASS;
    private static readonly string TITLECLASS = HaWeb.Settings.CSSClasses.TITLECLASS;
    private static readonly string ENTRYCLASS = HaWeb.Settings.CSSClasses.ENTRYCLASS;

    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( (x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LEMMACLASS))),
        ( (x, _) => x.Name == "title", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS))),
        ( (x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS))),
        ( (x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, ENTRYCLASS))),
    };

    public static readonly TagFuncList CTagRules = new TagFuncList() {
        ( (x, _) => x.Name == "lemma", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( (x, _) => x.Name == "title", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( (x, _) => x.Name == "titel", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( (x, _) => x.Name == "eintrag", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
    };

    public static readonly TagFuncList STagRules = new TagFuncList() {
        ( (x, _) => x.Name == "line", (sb, tag, _) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("br")) )
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( (x, _) => true, ( sb, txt, _) => sb.Append(txt.Value) )
    };

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( (x, _) => true, ( sb, txt, _) => sb.Append(txt.Value) )
    };
} 