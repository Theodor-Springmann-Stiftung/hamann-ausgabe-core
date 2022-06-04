namespace HaWeb.Settings.ParsingRules;
using System.Text;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.XMLHelper<HaWeb.Settings.ParsingState.LinkState>>)>;

public static class LinkRules {
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;
    private static readonly string INSERTEDLEMMACLASS = HaWeb.Settings.CSSClasses.INSERTEDLEMMACLASS;
    private static readonly string TITLECLASS = HaWeb.Settings.CSSClasses.TITLECLASS;

    // Parsing Rules for inserting lemmas
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( (x, _) => x.Name == "lemma", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, INSERTEDLEMMACLASS)) ),
        ( (x, _) => x.Name == "titel", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS)) ),
        ( (x, _) => x.Name == "title", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS)) )
    };

    public static readonly TagFuncList CTagRules = new TagFuncList() {
        ( (x, _) => x.Name == "lemma", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( (x, _) => x.Name == "titel", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)) ),
        ( (x, _) => x.Name == "title", (strbd, _, _) => strbd.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT)) )
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( (x, _) => true, (strbd, txt, _) => strbd.Append(txt.Value))
    };
}