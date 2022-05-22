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

public static class BriefeHelpers
{
    public static string CreateLetter(ILibrary lib, IReaderService readerService, Meta meta, Letter letter, IEnumerable<Marginal>? marginals)
    {
        var rd = readerService.RequestStringReader(letter.Element);
        var letterState = new LetterState(lib, readerService, meta, marginals);
        new HaWeb.HTMLParser.XMLHelper<LetterState>(letterState, rd, letterState.sb_lettertext, LetterRules.OTagRules, LetterRules.STagRules, LetterRules.CTagRules, LetterRules.TextRules, LetterRules.WhitespaceRules);
        // new HaWeb.HTMLParser.XMLHelper<LetterState>(letterState, rd, letterState.sb_lettertext, null, LetterRules.STagRulesLineCount);

        // if (marginals != null && marginals.Any())
        // {
        //     new HaWeb.HTMLParser.XMLHelper<LetterState>(letterState, rd, letterState.sb_lettertext, null, LetterRules.STagRulesMarginals);
        // }
        rd.Read();

        return letterState.sb_lettertext.ToString();
    }

    public static string CreateTraditions(ILibrary lib, IReaderService readerService, IEnumerable<Marginal>? marginals, Tradition tradition)
    {
        var rd = readerService.RequestStringReader(tradition.Element);
        var traditionState = new TraditionState(lib, rd, marginals);
        new HaWeb.HTMLParser.XMLHelper<TraditionState>(traditionState, rd, traditionState.sb_tradition, TraditionRules.OTagRulesInitial, null, TraditionRules.CTagRulesInitial, null, null);
        new HaWeb.HTMLParser.XMLHelper<TraditionState>(traditionState, rd, traditionState.sb_tradition, TraditionRules.OTagRules, TraditionRules.STagRulesInitial, TraditionRules.CTagRules, TraditionRules.TextRules, TraditionRules.WhitespaceRules);
        new HaWeb.HTMLHelpers.LinkHelper(lib, rd, traditionState.sb_tradition);
        rd.Read();
        return traditionState.sb_tradition.ToString();
    }

    public static List<string> CreateEdits(ILibrary lib, IReaderService readerService, IEnumerable<Editreason> editreasons)
    {
        var editstrings = new List<string>();
        var editsState = new EditState();
        foreach (var edit in editreasons)
        {
            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("div", "edit"));
            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "pageline"));
            var currstring = edit.StartPage + "/" + edit.StartLine;
            if (edit.StartPage != edit.EndPage)
                currstring += "–" + edit.EndPage + "/" + edit.EndLine;
            else if (edit.StartLine != edit.EndLine)
                currstring += "–" + edit.EndLine;
            editsState.sb_edits.Append(currstring + "&emsp;");
            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            if (!String.IsNullOrWhiteSpace(edit.Reference))
            {
                var sb2 = new StringBuilder();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                var rd = readerService.RequestStringReader(edit.Reference);
                new HaWeb.HTMLParser.XMLHelper<EditState>(editsState, rd, sb2, EditRules.OTagRules, null, EditRules.CTagRules, EditRules.TextRules, EditRules.WhitespaceRules);
                rd.Read();
                sb2.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                if ((edit.StartPage != edit.EndPage || edit.StartLine != edit.EndLine) && XElement.Parse(sb2.ToString()).Value.ToString().Length >= 60)
                {
                    var text = XElement.Parse(sb2.ToString()).Value.ToString();
                    text = text.ToString().Split(' ').Take(1).First() + " [&#x2026;] " + text.ToString().Split(' ').TakeLast(1).First();
                    editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "reference"));
                    editsState.sb_edits.Append(text);
                    editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
                }
                else
                    editsState.sb_edits.Append(sb2);
            }
            if (!String.IsNullOrWhiteSpace(edit.Element))
            {
                editsState.sb_edits.Append("&emsp;");
                editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateElement("span", "corrections"));
                var rd = readerService.RequestStringReader(edit.Element);
                new HaWeb.HTMLParser.XMLHelper<EditState>(editsState, rd, editsState.sb_edits, EditRules.OTagRules, EditRules.STagRules, EditRules.CTagRules, EditRules.TextRules, EditRules.WhitespaceRules);
                rd.Read();
                editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("span"));
            }
            editsState.sb_edits.Append(HaWeb.HTMLHelpers.TagHelpers.CreateEndElement("div"));
            editstrings.Add(editsState.sb_edits.ToString());
            editsState.sb_edits.Clear();
        }
        return editstrings;
    }

    public static List<string> CreateHands(ILibrary lib, ImmutableList<Hand> hands)
    {
        var handstrings = new List<string>();
        foreach (var hand in hands.OrderBy(x => x.StartPage.Length).ThenBy(x => x.StartPage).ThenBy(x => x.StartLine.Length).ThenBy(x => x.StartLine))
        {
            var currstring = hand.StartPage + "/" + hand.StartLine;
            if (hand.StartPage != hand.EndPage)
                currstring += "–" + hand.EndPage + "/" + hand.EndLine;
            else
                if (hand.StartLine != hand.EndLine)
                currstring += "–" + hand.EndLine;
            var persons = lib.HandPersons.Where(x => x.Key == hand.Person);
            if (persons.Any())
            {
                currstring += " " + persons.FirstOrDefault().Value.Name;
                handstrings.Add(currstring);
            }
        }
        return handstrings;
    }
}