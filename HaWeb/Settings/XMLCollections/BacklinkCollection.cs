namespace HaWeb.Settings.XMLCollections;
using HaWeb.Models;
using System.Xml.Linq;

public class BackLinkCollection : HaWeb.XMLParser.IXMLCollection {
    private static readonly Random _random = new Random();
    public string Key { get; } = "backlinks";
    public string[] xPath { get; } = new string[] { "/opus/data/marginalien/marginal//link", "/opus/marginalien/marginal//link", "/opus/kommentare/kommentar//link", "/opus/data/kommentare/kommentar//link" };
    public Func<XElement, string?> GenerateKey { get; } = GetKey;
    public Func<XElement, IDictionary<string, string>?>? GenerateDataFields { get; } = GetDataFields;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, ILookup<string, CollectedItem>>?>? GroupingsGeneration { get; } = GetLookups;
    public Func<IEnumerable<CollectedItem>, IDictionary<string, IEnumerable<CollectedItem>>?>? SortingsGeneration { get; } = null;
    public HaWeb.XMLParser.IXMLCollection[]? SubCollections { get; } = null;
    public bool Searchable { get; } = true;

    public static Func<XElement, string?> GetKey { get; } = (elem) => {
        var letter = (string?)elem.Attribute("letter");
        var page = (string?)elem.Attribute("page");
        var line = (string?)elem.Attribute("line");
        if (letter == null) return null;
        var index = letter + "-" + page ?? "" + "-" + line ?? "";
        if (String.IsNullOrWhiteSpace(index)) return null;
        return index + _random.Next().ToString();
    };

    public static IDictionary<string, string>? GetDataFields(XElement element) {
        var res = new Dictionary<string, string>();
        var marg = element.Ancestors("marginal").First();
        var letter = (string?)marg.Attribute("letter");
        var page = (string?)marg.Attribute("page");
        var line = (string?)marg.Attribute("line");
        var refere = (string?)element.Attribute("ref");
        var subref = (string?)element.Attribute("subref");
        if (letter == null || (refere == null && subref == null)) return null;
        if (subref != null) res.Add("ref", subref);
        else res.Add("ref", refere!);
        res.Add("letter", letter);
        if (page != null) res.Add("page", page);
        if (line != null) res.Add("line", line);
        return res;
    }

    public static IDictionary<string, ILookup<string, CollectedItem>>? GetLookups(IEnumerable<CollectedItem> items) {
        var res = new Dictionary<string, ILookup<string, CollectedItem>>();
        var refs = items.Where(x => x["ref"] != null);
        if (refs == null || !refs.Any()) return null;
        res.Add("ref", refs.ToLookup(x => x["ref"])!);
        return res;
    }
}