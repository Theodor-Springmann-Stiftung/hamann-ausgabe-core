namespace HaWeb.Settings.ParsingState;

using HaDocument.Interfaces;
using HaXMLReader.Interfaces;
using HaXMLReader.EvArgs;
using HaDocument.Models;
using System.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

public class TextState : HaWeb.HTMLParser.IState {
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
    private string? _currpage;
    internal string? currpage { 
        get => _currpage;
        set {
            if (Startpage == null)
                Startpage = value;
            _currpage = value;
            pagebreak = true;
    }}
    // Does the container need a min-width, so percentages are useful (tables)
    internal bool minwidth;
    // Did a pagebreak just occur?
    internal bool pagebreak = false;
    // Are we in line-counted territory?
    internal bool activelinecount;

    // Results
    internal StringBuilder sb;
    internal List<(string, string, string)>? ParsedMarginals;
    internal string? Startline;
    internal string? Startpage = null;

    public TextState(ILibrary lib, IReaderService readerService, Meta meta, IEnumerable<Marginal>? marginals, IEnumerable<Hand>? hands, IEnumerable<Editreason>? edits) {
        Lib = lib;
        ReaderService = readerService;
        Meta = meta;
        Marginals = marginals;
        Hands = hands;
        Edits = edits;
        SetupState();
    }


    public void SetupState() {
        sb = new StringBuilder();
        active_skipwhitespace = true;
        currline = "-1";
        mustwrap = (false, false);
        minwidth = false;
        activelinecount = true;

        // Initialize State
        if (Meta.ZH != null && !String.IsNullOrWhiteSpace(Meta.ZH.Page)) {
            currpage = Meta.ZH.Page;
        }
    }
}