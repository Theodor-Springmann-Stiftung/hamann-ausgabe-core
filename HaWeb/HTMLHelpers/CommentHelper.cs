namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using HaWeb.Settings.ParsingRules;
using HaWeb.Settings.ParsingState;
using System.Text;

public static class CommentHelpers
{
    private static readonly string DEFAULTELEMENT = HaWeb.Settings.HTML.DEFAULTELEMENT;
    private static readonly string BACKLINKSCLASS = HaWeb.Settings.CSSClasses.BACKLINKSCLASS;
    private static readonly string LETLINKCLASS = HaWeb.Settings.CSSClasses.LETLINKCLASS;
    private static readonly string COMMENTHEADCLASS = HaWeb.Settings.CSSClasses.COMMENTHEADCLASS;
    private static readonly string BACKLINKSHKBCLASS = HaWeb.Settings.CSSClasses.BACKLINKSHKBCLASS;

    public static string CreateHTML(ILibrary lib, IReaderService readerService, Comment comment, string category, CommentType type)
    {
        var sb = new StringBuilder();
        var rd = readerService.RequestStringReader(comment.Lemma);
        var commentState = new CommentState(category, type);
        new HTMLParser.XMLHelper<CommentState>(commentState, rd, sb, CommentRules.OTagRules, CommentRules.STagRules, CommentRules.CTagRules, CommentRules.TextRules, CommentRules.WhitespaceRules);
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
        rd = readerService.RequestStringReader(comment.Entry);
        new HTMLParser.XMLHelper<CommentState>(commentState, rd, sb, CommentRules.OTagRules, CommentRules.STagRules, CommentRules.CTagRules, CommentRules.TextRules, CommentRules.WhitespaceRules);
        new HTMLHelpers.LinkHelper(lib, rd, sb);
        rd.Read();
        return sb.ToString();
    }
}