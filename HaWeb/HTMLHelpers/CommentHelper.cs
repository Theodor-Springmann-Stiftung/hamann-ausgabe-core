namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;

public static class CommentHelpers
{

    private static readonly string DEFAULTELEMENT = HaWeb.Settings.ParsingSettings.DEFAULTELEMENT;
    private static readonly string LEMMACLASS = HaWeb.Settings.CSSClasses.LEMMACLASS;
    private static readonly string TITLECLASS = HaWeb.Settings.CSSClasses.TITLECLASS;
    private static readonly string BACKLINKSCLASS = HaWeb.Settings.CSSClasses.BACKLINKSCLASS;
    private static readonly string LETLINKCLASS = HaWeb.Settings.CSSClasses.LETLINKCLASS;
    private static readonly string COMMENTHEADCLASS = HaWeb.Settings.CSSClasses.COMMENTHEADCLASS;
    private static readonly string COMMENTBODYCLASS = HaWeb.Settings.CSSClasses.COMMENTBODYCLASS;
    // Parsing Rules
    private static readonly List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _OTagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, LEMMACLASS))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement(DEFAULTELEMENT, TITLECLASS)))
    };

    private static readonly List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _CTagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement(DEFAULTELEMENT)))
    };

    private static readonly List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _STagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "line", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("br")) )
    };

    private static readonly List<(Func<Text, bool>, Action<StringBuilder, Text>)> _TextFuncs = new List<(Func<Text, bool>, Action<StringBuilder, Text>)>() {
        ( x => true, ( sb, txt ) => sb.Append(txt.Value) )
    };

    private static readonly List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)> _WhitespaceFuncs = new List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)>() {
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
                        sb.Append("HKB&nbsp;");
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