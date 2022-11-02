namespace HaWeb.SearchHelpers;
using System.Text;
using System.Web;

using TagFuncList = List<(Func<HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Tag, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;
using TextFuncList = List<(Func<HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Text, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;
using WhitespaceFuncList = List<(Func<HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.LineXMLHelper<SearchState>, bool>, Action<System.Text.StringBuilder, HaXMLReader.EvArgs.Whitespace, HaWeb.HTMLParser.LineXMLHelper<SearchState>>)>;


public class SearchRules {
    public static readonly TagFuncList OTagRules = new TagFuncList() {
        ( (x, _) => x.Name.ToLower() == "kommentar" || x.Name.ToLower() == "subsection", (_, tag, reader) => reader.State.CurrentIdentifier = tag["id"]),
    };

    public static readonly TextFuncList TextRules = new TextFuncList() {
        ( (x, _) => true, (sb, text, reader) => {
            var t = text.Value;
            if (reader.State.Normalize)
                t = HaWeb.SearchHelpers.StringHelpers.NormalizeWhiteSpace(t);
            sb.Append(t);
            var sw = reader.State.SearchWord;
            if (sb.Length >= sw.Length) {
                if (sb.ToString().ToUpperInvariant().Contains(sw)) {
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