namespace HaWeb.Settings.ParsingState;

using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

public class LetterState : HaWeb.HTMLParser.IState {
     // Input
    internal ILibrary Lib;
    internal IReaderService ReaderService;
    internal Meta Meta;
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

    // Results
    internal StringBuilder sb_lettertext;
    internal List<(string, string, string)>? ParsedMarginals;
    internal string Startline;

    public LetterState(ILibrary lib, IReaderService readerService, Meta meta, IEnumerable<Marginal>? marginals, IEnumerable<Hand>? hands, IEnumerable<Editreason>? edits) {
        Lib = lib;
        ReaderService = readerService;
        Meta = meta;
        Marginals = marginals;
        Hands = hands;
        Edits = edits;
        SetupState();
    }


    public void SetupState() {
        sb_lettertext = new StringBuilder();
        active_skipwhitespace = true;
        currline = "-1";
        currpage = "";
        mustwrap = (false, false);
        minwidth = false;

        // Initialize State
        if (Meta.ZH != null) {
            currpage = Meta.ZH.Page;
        }
    }
}