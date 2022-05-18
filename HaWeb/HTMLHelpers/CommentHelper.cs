namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;

// Type aliasses for incredible long types
using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace>)>;

public static class CommentHelpers
{

    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;
    private static readonly string LEMMACLASS = HaWeb.Settings.CSSClasses.LEMMACLASS;
    private static readonly string TITLECLASS = HaWeb.Settings.CSSClasses.TITLECLASS;
    private static readonly string BACKLINKSCLASS = HaWeb.Settings.CSSClasses.BACKLINKSCLASS;
    private static readonly string LETLINKCLASS = HaWeb.Settings.CSSClasses.LETLINKCLASS;
    private static readonly string COMMENTHEADCLASS = HaWeb.Settings.CSSClasses.COMMENTHEADCLASS;
    private static readonly string COMMENTBODYCLASS = HaWeb.Settings.CSSClasses.COMMENTBODYCLASS;
    private static readonly string BACKLINKSHKBCLASS = HaWeb.Settings.CSSClasses.BACKLINKSHKBCLASS;
    
    // Parsing Rules
    private static readonly TagFuncList _OTagFuncs = new TagFuncList() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LEMMACLASS))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS)))
    };

    private static readonly TagFuncList _CTagFuncs = new TagFuncList() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)))
    };

    private static readonly TagFuncList _STagFuncs = new TagFuncList() {
        ( x => x.Name == "line", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("br")) )
    };

    private static readonly TextFuncList _TextFuncs = new TextFuncList() {
        ( x => true, ( sb, txt ) => sb.Append(txt.Value) )
    };

    private static readonly WhitespaceFuncList _WhitespaceFuncs = new WhitespaceFuncList() {
        ( x => true, ( sb, txt ) => sb.Append(txt.Value) )
    };

    public static string CreateHTML(ILibrary lib, IReaderService readerService, Comment comment)
    {
        StringBuilder sb = new StringBuilder();
        var rd = readerService.RequestStringReader(comment.Lemma);
        new HTMLHelpers.XMLHelper(rd, sb, _OTagFuncs, _STagFuncs, _CTagFuncs, _TextFuncs, _WhitespaceFuncs);
        sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, COMMENTHEADCLASS, comment.Index));
        new HTMLHelpers.LinkHelper(lib, rd, sb);
        rd.Read();
        var backlinks = lib.Backlinks.ContainsKey(comment.Index) ? lib.Backlinks[comment.Index]
            .Where(x => lib.Metas.ContainsKey(x.Letter))
            .OrderBy(x => lib.Metas[x.Letter].Sort)
            .ThenBy(x => lib.Metas[x.Letter].Order) : null;
        if (backlinks != null)
        {
            sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, BACKLINKSCLASS));
            var arrow = false;
            foreach (var blk in backlinks)
            {
                var let = lib.Metas.ContainsKey(blk.Letter) ? lib.Metas[blk.Letter] : null;
                if (let != null)
                {
                    if (!arrow)
                    {
                        sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, BACKLINKSHKBCLASS));
                        sb.Append("HKB&nbsp;");
                        sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
                        arrow = true;
                    }
                    sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", LETLINKCLASS, "/Briefe/" + let.Autopsic + "#" + blk.Page + "-" + blk.Line));
                    var linkstring = "";
                    var pglnstring = "";
                    linkstring += let.Autopsic;
                    pglnstring += "&nbsp;(&#8239;" + blk.Page + "/" + blk.Line + "&#8239;)";
                    linkstring += pglnstring;
                    sb.Append(linkstring);
                    if (blk != backlinks.Last())
                        sb.Append(", ");
                    sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("a"));
                }
            }
            sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        }
        sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, COMMENTBODYCLASS));
        rd = readerService.RequestStringReader(comment.Entry);
        new HTMLHelpers.XMLHelper(rd, sb, _OTagFuncs, _STagFuncs, _CTagFuncs, _TextFuncs, _WhitespaceFuncs);
        new HTMLHelpers.LinkHelper(lib, rd, sb);
        rd.Read();
        sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT));
        return sb.ToString();
    }
}