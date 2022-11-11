namespace HaWeb.Settings.ParsingRules;
using System.Text;
using System.Text.RegularExpressions;

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
        ( (x, _) => true, ( sb, txt, reader) => {
            sb.Append(txt.Value);
            if (reader.State.Category == "bibel" && reader.State.Type == HaWeb.Settings.ParsingState.CommentType.Subcomment &&
            reader.OpenTags.Any() && reader.OpenTags.Last().Name == "lemma" &&
                !txt.Value.Contains("Stücke zu") && !txt.Value.Contains("ZusDan")) {
                var lnkstring = Regex.Replace(txt.Value, @"\s+", string.Empty);
                sb.Append(HTMLHelpers.TagHelpers.CreateCustomElement("a",
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "href", Value = "https://www.bibleserver.com/LUT/" + lnkstring},
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "target", Value = "_blank"},
                        new HaWeb.HTMLHelpers.TagHelpers.Attribute() { Name = "rel", Value = "noopener noreferrer"}));
                sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"12\" height=\"12\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" class=\"feather feather-external-link\"><path d=\"M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6\"></path><polyline points=\"15 3 21 3 21 9\"></polyline><line x1=\"10\" y1=\"14\" x2=\"21\" y2=\"3\"></line></svg>");
                sb.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("a"));
            }
    })};

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( (x, _) => true, ( sb, txt, _) => sb.Append(txt.Value) )
    };
}