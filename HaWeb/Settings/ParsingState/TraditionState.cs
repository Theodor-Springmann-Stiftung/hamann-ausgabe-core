namespace HaWeb.Settings.ParsingState;
using System.Text;
using HaXMLReader.Interfaces;
using HaDocument.Models;
using HaDocument.Interfaces;
using System.Collections.Immutable;

public class TraditionState : HaWeb.HTMLParser.IState {

    internal ILibrary Lib;
    internal IReaderService ReaderService;

    internal IEnumerable<Marginal>? Marginals;
    internal IEnumerable<Hand>? Hands;
    internal IEnumerable<Editreason>? Edits;

    // State
    // Must we skip all of the upcoming whitespace?
    internal bool active_skipwhitespace;
    // Is there a semantically important line break, left or right of the current line?
    internal (bool, bool) mustwrap;
    // What's the current line?
    internal string currline;
    // What's the current page?
    internal string currpage;
    // Does the container need a min-widt, so percentages are useful (tables)
    internal bool minwidth;
    // Is there an active_zhtext
    internal bool active_trad;

    internal StringBuilder sb_tradition;     // Überlieferung
    internal List<(string, string, string)>? ParsedMarginals;
    internal string Startline;

    internal IReader rd_tradition;

    public TraditionState(ILibrary lib, IReader reader, IReaderService readerService, IEnumerable<Marginal>? marginals, IEnumerable<Hand>? hands, IEnumerable<Editreason>? edits) {
        Lib = lib;
        rd_tradition = reader;
        Marginals = marginals;
        ReaderService = readerService;
        Hands = hands;
        Edits = edits;
        SetupState();
    }

    public void SetupState() {
        sb_tradition = new StringBuilder();

        active_trad = false;
        active_skipwhitespace = true;
        currline = "-1";
        currpage = string.Empty;
    }
}