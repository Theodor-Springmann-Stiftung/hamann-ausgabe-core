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

    // State
    internal bool active_del;
    internal bool active_skipwhitespace;
    internal string currline;
    internal string currpage;
    internal string oldpage;
    internal int commid;

    // Parsing-Combinations
    internal StringBuilder sb_lettertext;    // Hauptext
    internal StringBuilder sb_linecount;     // Linke Spalte (Zeilenzählung)
    internal StringBuilder sb_marginals;     // Rechte Spalte (Kommentare)


    public LetterState(ILibrary lib, IReaderService readerService, Meta meta, IEnumerable<Marginal>? marginals) {
        Lib = lib;
        ReaderService = readerService;
        Meta = meta;
        Marginals = marginals;

        SetupState();
    }


    public void SetupState() {
        sb_lettertext = new StringBuilder();
        sb_linecount = new StringBuilder();
        sb_marginals = new StringBuilder();

        active_skipwhitespace = true;
        currline = "-1";
        currpage = "";
        oldpage = "";
        commid = 1;

        // Initialize State
        if (Meta.ZH != null) {
            currpage = Meta.ZH.Page;
        }
    }
}