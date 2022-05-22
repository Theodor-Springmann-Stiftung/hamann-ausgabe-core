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
    internal bool active_trad;

    internal StringBuilder sb_tradition;     // Überlieferung
    internal StringBuilder sb_trad_zhtext;   // Überlieferung, ZHText

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
       
        active_trad = false;
        active_del = false;
        active_skipwhitespace = true;
        active_firstedit = true;
        currline = "-1";
        currpage = "";
    }
}