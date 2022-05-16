namespace HaWeb.Models;
using HaDocument.Models;
using HaDocument.Interfaces;
using System.Text;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;

public class CommentModel
{
    public Comment Comment { get; private set; }
    public string? ParsedComment { get; private set; }
    public List<CommentModel>? SubComments { get; private set; }

    // Parsing Rules
    private static List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _OTagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("div", "lemma"))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("div", "title"))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("div", "title")))
    };

    private static List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _CTagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "lemma", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("div"))),
        ( x => x.Name == "title", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("div"))),
        ( x => x.Name == "titel", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("div")))
    };

    private static List<(Func<Tag, bool>, Action<StringBuilder, Tag>)> _STagFuncs = new List<(Func<Tag, bool>, Action<StringBuilder, Tag>)>() {
        ( x => x.Name == "line", (sb, tag) => sb.Append(HTMLHelpers.TagHelpers.CreateElement("br")) )
    };

    private static List<(Func<Text, bool>, Action<StringBuilder, Text>)> _TextFuncs = new List<(Func<Text, bool>, Action<StringBuilder, Text>)>() {
        ( x => true, ( sb, txt ) => sb.Append(txt.Value) )
    };

    private static List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)> _WhitespaceFuncs = new List<(Func<Whitespace, bool>, Action<StringBuilder, Whitespace>)>() {
        ( x => true, ( sb, txt ) => sb.Append(txt.Value) )
    };

    public CommentModel(Comment comment)
    {
        this.Comment = comment;
        if (comment.Kommentare != null && comment.Kommentare.Any())
        {
            SubComments = comment.Kommentare.Select(x => new CommentModel(x.Value)).OrderBy(x => x.Comment.Order).ToList();
        }
    }

    public string returnHTML(ILibrary _lib, IReaderService _readerService)
    {
        StringBuilder sb = new StringBuilder();
        var rd = _readerService.RequestStringReader(Comment.Lemma);
        new HTMLHelpers.XMLHelper(rd, sb, _OTagFuncs, _STagFuncs, _CTagFuncs, _TextFuncs, _WhitespaceFuncs);
        sb.Append(HTMLHelpers.TagHelpers.CreateElement("div", "lemma", Comment.Index));
        new HTMLHelpers.LinkHelper(_lib, rd, sb);
        rd.Read();
        var backlinks = _lib.Backlinks.ContainsKey(Comment.Index) ? _lib.Backlinks[Comment.Index]
            .Where(x => _lib.Metas.ContainsKey(x.Letter))
            .OrderBy(x => _lib.Metas[x.Letter].Sort)
            .ThenBy(x => _lib.Metas[x.Letter].Order) : null;
        if (backlinks != null)
        {
            sb.Append(HTMLHelpers.TagHelpers.CreateElement("div", "backlinks"));
            var arrow = false;
            foreach (var blk in backlinks)
            {
                var let = _lib.Metas.ContainsKey(blk.Letter) ? _lib.Metas[blk.Letter] : null;
                if (let != null)
                {
                    if (!arrow)
                    {
                        sb.Append("&emsp;&rarr;&nbsp;");
                        sb.Append("HKB&nbsp;");
                        arrow = true;
                    }
                    sb.Append(HTMLHelpers.TagHelpers.CreateElement("a", "backlink", "/Briefe/" + let.Autopsic + "#" + blk.Page + "-" + blk.Line));
                    var linkstring = "";
                    var pglnstring = "";
                    linkstring += let.Autopsic;
                    pglnstring += "&nbsp;(&#8239;" + blk.Page + "/" + blk.Line + "&#8239;)";
                    linkstring += pglnstring;
                    sb.Append(linkstring);
                    if (blk != backlinks.Last())
                        sb.Append(",&emsp;");
                    sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("a"));
                }
            }
            sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("div"));
        }
        sb.Append(HTMLHelpers.TagHelpers.CreateEndElement("div"));
        rd = _readerService.RequestStringReader(Comment.Entry);
        new HTMLHelpers.XMLHelper(rd, sb, _OTagFuncs, _STagFuncs, _CTagFuncs, _TextFuncs, _WhitespaceFuncs);
        new HTMLHelpers.LinkHelper(_lib, rd, sb);
        rd.Read();

        if (SubComments != null && SubComments.Any())
        {
            foreach (var k in SubComments)
            {
                k.SetHTML(_lib, _readerService);
            }
        }
        return sb.ToString();
    }

    public void SetHTML(ILibrary _lib, IReaderService _readerService) {
        ParsedComment = returnHTML(_lib, _readerService);
    }
}