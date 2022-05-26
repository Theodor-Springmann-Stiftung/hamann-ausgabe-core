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

    internal List<(string, string, string)> ParsedMarginals;

    // State
    internal bool active_del;
    internal bool active_skipwhitespace;
    internal string currline;
    internal string currpage;

    // Parsing-Combinations
    internal StringBuilder sb_lettertext;


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
        ParsedMarginals = new List<(string, string, string)>();
        sb_lettertext = new StringBuilder();
        active_skipwhitespace = true;
        currline = "-1";
        currpage = "";

        // Initialize State
        if (Meta.ZH != null) {
            currpage = Meta.ZH.Page;
        }
    }
}