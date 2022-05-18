namespace HaWeb.Settings;

public static class CSSClasses {
    public const string LEMMACLASS = "ha-lemma"; // XML: <lemma>
    public const string TITLECLASS = "ha-title"; // XML: <title>
    public const string BACKLINKSCLASS = "ha-letlinks"; // Collection containing links to references in letters
    public const string COMMENTHEADCLASS = "ha-commenthead"; // Head of a comment, containing lemma and backlinks
    public const string COMMENTBODYCLASS = "ha-commentbody"; // Body of a comment, contasining the text

    public const string LETLINKCLASS = "ha-letlink"; // XML: <link>
    public const string REFLINKCLASS = "ha-reflink"; // XML <intlink>
    public const string WWWLINKCLASS = "ha-wwwlink"; // XML: <wwwlink>

    public const string INSERTEDLEMMACLASS = "ha-insertedlemma"; // XML <link linktext="true"></link>
    
}