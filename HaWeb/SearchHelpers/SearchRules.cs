namespace HaWeb.SearchHelpers;
using System.Text;
using System.Web;
using HaDocument.Models;
using HaDocument.Interfaces;
using System.Xml.Linq;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;


public class SearchRules {
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( (x, _) => x.Name.ToLower() == "kommentar" || x.Name.ToLower() == "subsection", (_, tag, reader) => reader.State.CurrentIdentifier = tag["id"]),
        ( (x, reader) => (x.Name.ToLower() == "link" && reader.State.Lib != null), (sb, tag, reader) => {
            // LINKTEXT NEVER GETS TO TRUE
            Comment? comment = null;
            if (tag.Values.ContainsKey("subref") && reader.State.Lib.SubCommentsByID.ContainsKey(tag["subref"]))
                comment = reader.State.Lib.SubCommentsByID[tag["subref"]];
            else if (tag.Values.ContainsKey("ref"))
                if (reader.State.Lib.Comments.ContainsKey(tag["ref"]))
                    comment = reader.State.Lib.Comments[tag["ref"]];
                else if (reader.State.Lib.SubCommentsByID.ContainsKey(tag["ref"]))
                    comment = reader.State.Lib.SubCommentsByID[tag["ref"]];
            if (comment != null) {
                var t = String.Empty;
                var sw = reader.State.SearchWord;
                if (!String.IsNullOrWhiteSpace(comment.Lemma))
                    t = XElement.Parse(comment.Lemma).Value;
                if (reader.State.Normalize)
                    t = HaWeb.SearchHelpers.StringHelpers.NormalizeWhiteSpace(t);
                if (tag["linktext"] != "false") {
                    sb.Append(t.ToUpperInvariant());
                    if (sb.Length >= sw.Length) {
                        if (sb.ToString().Contains(sw)) {
                            if (reader.State.Results == null)
                                reader.State.Results = new List<(string Page, string Line, string Identifier)>();
                            reader.State.Results.Add((reader.CurrentPage, reader.CurrentLine, reader.State.CurrentIdentifier));
                        }
                        sb.Remove(0, sb.Length - sw.Length);
                    }
                } 
                // Enable, if zou want unparsed context tto be conidered when searching.
                // else {
                //     if (t.ToUpperInvariant().Contains(sw)) {
                //         if (reader.State.Results == null)
                //                 reader.State.Results = new List<(string Page, string Line, string Identifier)>();
                //         reader.State.Results.Add((reader.CurrentPage, reader.CurrentLine, reader.State.CurrentIdentifier));
                //     }
                // }
            }
        })
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( (x, _) => true, (sb, text, reader) => {
            var t = text.Value;
            if (reader.State.Normalize)
                t = HaWeb.SearchHelpers.StringHelpers.NormalizeWhiteSpace(t);
            sb.Append(t.ToUpperInvariant());
            var sw = reader.State.SearchWord;
            if (sb.Length >= sw.Length) {
                if (sb.ToString().Contains(sw)) {
                    if (reader.State.Results == null)
                        reader.State.Results = new List<(string Page, string Line, string Identifier)>();
                    reader.State.Results.Add((reader.CurrentPage, reader.CurrentLine, reader.State.CurrentIdentifier));
                }
                sb.Remove(0, sb.Length - sw.Length);
            }
        })
    };

    public static readonly WhitespaceFuncList WhitespaceRules = new WhitespaceFuncList() {
        ( (x, _) => true, (sb, text, reader) => {
            var t = text.Value;
            if (reader.State.Normalize)
                t = HaWeb.SearchHelpers.StringHelpers.NormalizeWhiteSpace(t);
            sb.Append(t);
            var sw = reader.State.SearchWord;
            if (sb.Length >= sw.Length) {
                if (sb.ToString().Contains(sw)) {
                    if (reader.State.Results == null)
                        reader.State.Results = new List<(string Page, string Line, string Identifier)>();
                    reader.State.Results.Add((reader.CurrentPage, reader.CurrentLine, reader.State.CurrentIdentifier));
                }
                sb.Remove(0, sb.Length - sw.Length);
            }
        })
    };
}