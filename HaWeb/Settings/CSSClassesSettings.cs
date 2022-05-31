namespace HaWeb.Settings;

public static class CSSClasses {
    // Classes generated by parsing the XML:
    // Root elements
    // TODO We dont parse all the root elements, so <letter>, etc. as of right now
    public const string COMMENTCLASS = "ha-comment"; // XML: <kommentar>
    public const string EDITREASONCLASS = "ha-editreason"; // XML: <editreason>
    public const string SUBSECTIONCLASS = "ha-subsection"; // XML: <subsection>
    public const string TRADITIONCLASS = "ha-tradition"; // XML: <tradition>
    public const string MARGINALCLASS = "ha-marginal"; // XML: <marginal>
    public const string LETTERCLASS = "ha-lettertext"; // XML: <lemma>

    // Comments:
    public const string LEMMACLASS = "ha-lemma"; // XML: <lemma>
    public const string TITLECLASS = "ha-title"; // XML: <title>
    public const string ENTRYCLASS = "ha-entry"; // XML: <eintrag>
    public const string BACKLINKSCLASS = "ha-letlinks"; // Collection containing links to references in letters
    public const string BACKLINKSHKBCLASS = "ha-hkb"; // HKB-Text infront of l;ink collection
    public const string COMMENTHEADCLASS = "ha-commenthead"; // Head of a comment, containing lemma and backlinks

    // Links:
    public const string LETLINKCLASS = "ha-letlink"; // XML: <link>
    public const string REFLINKCLASS = "ha-reflink"; // XML <intlink>
    public const string WWWLINKCLASS = "ha-wwwlink"; // XML: <wwwlink>
    public const string INSERTEDLEMMACLASS = "ha-insertedlemma"; // XML <link linktext="true"></link>

    // Hauptsächlich Brieftext:
    public const string ALIGNCENTERCLASS = "ha-aligncenter"; // XML: <align pos = center>
    public const string ALIGNRIGHTCLASS = "ha-alignright"; // XML: <align pos = rigth>
    public const string ADDEDCLASS = "ha-added"; // XML: <added>
    public const string SALCLASS = "ha-sal"; // XML: <sal>
    public const string AQCLASS = "ha-aq"; // XML: <aq>  
    public const string SUPERCLASS = "ha-super"; // XML: <super>
    public const string DELCLASS = "ha-del"; // XML: <del>
    public const string NRCLASS = "ha-nr"; // XML: 
    public const string NOTECLASS = "ha-note"; // XML: 
    public const string ULCLASS = "ha-ul"; // XML:   
    public const string ANCHORCLASS = "ha-anchor"; // XML: 
    public const string FNCLASS = "ha-fn"; // XML: 
    public const string DULCLASS = "ha-dul"; // XML: 
    public const string FULCLASS = "ha-ful"; // XML: 
    public const string UPCLASS = "ha-up"; // XML: 
    public const string SUBCLASS = "ha-sub"; // XML:
    public const string TULCLASS = "ha-tul"; // XML:
    public const string HEADERCLASS = "ha-textheader"; // XML:
    public const string HANDCLASS = "ha-hand";
    public const string TABLECLASS = "ha-table";
    public const string TABCLASS = "ha-hatab-"; // TODO: GEN 
    public const string CROSSEDDASHCLASS = "ha-diagdel";
    public const string TEXTCLASS = "ha-text";

    public const string BZGCLASS = "ha-bzg";
    public const string ZHCLASS = "ha-zh";
    public const string EMPHCLASS = "ha-emph";
    public const string APPCLASS = "ha-app";
    public const string MARGINGALBOXCLASS = "ha-marginalbox";
    public const string MARGINALLISTCLASS = "ha-marginallist";
    public const string TRADLINECOUNTCLASS = "ha-tradlinecount";
    public const string TRADCOMMENTCOLUMNCLASS = "ha-tradcommentcolumn";
    public const string TRADZHTEXTCLASS = "ha-tradzhtext";
    public const string TRADZHTEXTBOXCLASS = "ha-tradtextbox";

    // Zeilen:
    public const string ZHLINECLASS = "ha-zhline";
    public const string FIRSTLINECLASS ="ha-firstline";
    public const string ZHBREAKCLASS = "ha-zhbreak";
    public const string LINELINECLASS = "ha-hr";
    public const string LINEINDENTCLASS = "ha-indent-"; // TODO: GEN
    public const string ZHPAGECLASS = "ha-zhpage";
    public const string ZHLINECOUNTCLASS = "ha-linecount";
    public const string HIDDENZHLINECOUNT = "ha-hiddenlinecount";
    public const string FIRSTPAGECLASS = "ha-firstpage";
    public const string EMPTYLINECLASS = "ha-emptyline";

    // Marker
    public const string EDITMARKERCLASS = "ha-editmarker";
    public const string COMMENTMARKERCLASS = "ha-commentmarker";
    public const string HANDMARKERCLASS = "ha-handmarker";

    // TODO Classes used in Razor Pages:


    // TODO Classes used in Javascript:

    // TODO IDs used by JavaScript:


}