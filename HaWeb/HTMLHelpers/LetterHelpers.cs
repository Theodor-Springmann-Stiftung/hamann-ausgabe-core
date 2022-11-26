namespace HaWeb.HTMLHelpers;
using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

using HaWeb.Settings.ParsingState;
using HaWeb.Settings.ParsingRules;

public static class LetterHelpers {

    public static TextState ParseText(ILibrary lib, IReaderService readerService, string text, Meta meta, IEnumerable<Marginal>? marginals, IEnumerable<Hand>? hands, IEnumerable<Editreason>? edits)
        => ParseText(lib, readerService, XElement.Parse(text, LoadOptions.PreserveWhitespace), meta, marginals, hands, edits);

    public static TextState ParseText(ILibrary lib, IReaderService readerService, XElement text, Meta meta, IEnumerable<Marginal>? marginals, IEnumerable<Hand>? hands, IEnumerable<Editreason>? edits) {
        var rd = readerService.RequestReader(text);
        var state = new TextState(lib, readerService, meta, marginals, hands, edits);
        new HaWeb.HTMLParser.XMLHelper<TextState>(state, rd, state.sb, TextRules.OTagRules, TextRules.STagRules, TextRules.CTagRules, TextRules.TRules, TextRules.WhitespaceRules);
        new HaWeb.HTMLHelpers.LinkHelper(lib, rd, state.sb);
        rd.Read();
        return state;
    }

    public static List<(string, string, string, string, string, string)> CreateEdits(ILibrary lib, IReaderService readerService, IEnumerable<Editreason> editreasons) {
        editreasons = editreasons.OrderBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.StartPage)).ThenBy(x => HaWeb.HTMLHelpers.ConversionHelpers.RomanOrNumberToInt(x.StartLine));
        var editstrings = new List<(string, string, string, string, string, string)>();
        var editsState = new EditState();
        foreach (var edit in editreasons) {
            var currstring = edit.StartPage + "/" + edit.StartLine;
            var endstring = string.Empty;
            var refstring = string.Empty;
            if (edit.StartPage != edit.EndPage)
                endstring += edit.EndPage + "/" + edit.EndLine;
            else if (edit.StartLine != edit.EndLine)
                endstring += edit.EndLine;

            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "edit"));
            if (!String.IsNullOrWhiteSpace(edit.Reference)) {
                var sb2 = new StringBuilder();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                var rd = readerService.RequestStringReader(edit.Reference);
                new HaWeb.HTMLParser.XMLHelper<EditState>(editsState, rd, sb2, EditRules.OTagRules, null, EditRules.CTagRules, EditRules.TextRules, EditRules.WhitespaceRules);
                rd.Read();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                // Old: (edit.StartPage != edit.EndPage || edit.StartLine != edit.EndLine) && 
                if (XElement.Parse(sb2.ToString()).Value.ToString().Length >= 20) {
                    var text = XElement.Parse(sb2.ToString()).Value.ToString();
                    text = text.ToString().Split(' ').Take(1).First() + " [&#x2026;] " + text.ToString().Split(' ').TakeLast(1).First();
                    var sb3 = new StringBuilder();
                    sb3.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                    sb3.Append(text);
                    sb3.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                    refstring = sb3.ToString();
                } else
                    refstring = sb2.ToString();
            }
            if (!String.IsNullOrWhiteSpace(edit.Element)) {
                editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "corrections"));
                var rd = readerService.RequestStringReader(edit.Element);
                new HaWeb.HTMLParser.XMLHelper<EditState>(editsState, rd, editsState.sb_edits, EditRules.OTagRules, EditRules.STagRules, EditRules.CTagRules, EditRules.TextRules, EditRules.WhitespaceRules);
                rd.Read();
                editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            }
            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
            editstrings.Add((currstring, endstring, refstring, editsState.sb_edits.ToString(), edit.StartPage, edit.StartLine));
            editsState.sb_edits.Clear();
        }
        return editstrings;
    }

    public static List<(string, string, string, string, string)> CreateHands(ILibrary lib, ImmutableList<Hand> hands) {
        var handstrings = new List<(string, string, string, string, string)>();
        foreach (var hand in hands.OrderBy(x => x.StartPage.Length).ThenBy(x => x.StartPage).ThenBy(x => x.StartLine.Length).ThenBy(x => x.StartLine)) {
            var currstring = hand.StartPage + "/" + hand.StartLine;
            var endstring = string.Empty;
            var personstring = string.Empty;
            if (hand.StartPage != hand.EndPage)
                endstring += hand.EndPage + "/" + hand.EndLine;
            else
                if (hand.StartLine != hand.EndLine)
                endstring += hand.EndLine;
            var persons = lib.HandPersons.Where(x => x.Key == hand.Person);
            if (persons.Any()) {
                personstring += " " + persons.FirstOrDefault().Value.Name;
                handstrings.Add((currstring, endstring, personstring, hand.StartPage, hand.StartLine));
            }
        }
        return handstrings;
    }

    public static string CreateZHString(Meta meta) {
        var zhstrring = "ZH ";
        var a = 1;
        if (Int32.TryParse(meta.ZH.Volume, out a))
            zhstrring += HTMLHelpers.ConversionHelpers.ToRoman(a) + " ";
        zhstrring += meta.ZH.Page;
        return zhstrring;
    }
}