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

    internal bool active_del;
    internal bool active_skipwhitespace;
    internal bool active_firstedit;
    internal string currline;
    internal string currpage;
    internal string oldpage;
    internal int commid;
    internal bool active_trad;

    internal StringBuilder sb_tradition;     // Überlieferung
    internal StringBuilder sb_trad_zhtext;   // Überlieferung, ZHText
    internal StringBuilder sb_trad_left;     // Überlieferung ZHText linke Spalte (zeilenzählung)
    internal StringBuilder sb_trad_right;    // Überlieferung ZHText rechte Spalte (Kommentare)

    internal IReader rd_tradition;

    public TraditionState(ILibrary lib, IReader reader, IEnumerable<Marginal>? marginals) 
    {
        Lib = lib;
        rd_tradition = reader;
        Marginals = marginals;
        SetupState();
    }

    public void SetupState() {
        sb_tradition = new StringBuilder();
        sb_trad_zhtext = new StringBuilder();
        sb_trad_left = new StringBuilder();
        sb_trad_right = new StringBuilder();
       
        active_trad = false;
        active_del = false;
        active_skipwhitespace = true;
        active_firstedit = true;
        currline = "-1";
        currpage = "";
        oldpage = "";
        commid = 1;
    }
}